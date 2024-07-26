using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using CsvHelper;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Participants.Participant;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Auth;
using MCMS.Base.Data;
using MCMS.Base.Exceptions;
using MCMS.Base.JsonPatch;
using MCMS.Files.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace InfoEducatie.Main.InfoEducatieAdmin
{
    public class ImportService
    {
        private readonly IRepository<ProjectEntity> _projectsRepo;
        private readonly IRepository<ParticipantEntity> _participantsRepo;
        private readonly IRepository<CategoryEntity> _catsRepo;
        private readonly UserManager<User> _userManager;
        private List<ProjectEntity> _projectsCache;
        private List<ParticipantEntity> _participantsCache;
        private readonly HashSet<string> _seenProjectsIds = new();

        public ImportService(IRepository<ProjectEntity> projectsRepo, IRepository<ParticipantEntity> participantsRepo,
            UserManager<User> userManager, IRepository<CategoryEntity> catsRepo)
        {
            _projectsRepo = projectsRepo;
            _participantsRepo = participantsRepo;
            _userManager = userManager;
            _catsRepo = catsRepo;
            _projectsRepo.ChainQueryable(q => q.Include(p => p.Category));
            _participantsRepo.ChainQueryable(q => q
                .Include(p => p.User)
                .Include(p => p.Projects)
            );
        }

        private async Task PrepareCache()
        {
            _projectsRepo.SkipSaving = _participantsRepo.SkipSaving = true;
            _projectsCache = await _projectsRepo.GetAll();
            _participantsCache = await _participantsRepo.GetAll();
        }

        public CsvReader CsvReaderFromFile(FileEntity file)
        {
            var fs = new FileStream(file.PhysicalFullPath, FileMode.Open);
            var strReader = new StreamReader(fs);
            var reader = new CsvReader(strReader, CultureInfo.InvariantCulture);
            return reader;
        }

        public async Task<object> Import(FileEntity projectsCsvFile, FileEntity contestantsCsvFile,
            bool debug)
        {
            var projectsCsvReader = CsvReaderFromFile(projectsCsvFile);
            var contestantsCsvReader = CsvReaderFromFile(contestantsCsvFile);

            await projectsCsvReader.ReadAsync();
            projectsCsvReader.ReadHeader();
            await contestantsCsvReader.ReadAsync();
            contestantsCsvReader.ReadHeader();

            var missingProjectsFields = RequiredProjectsFields.Except(projectsCsvReader.HeaderRecord).ToList();
            if (missingProjectsFields.Any())
            {
                throw new KnownException("Missing columns from projects csv file: " +
                                         JsonConvert.SerializeObject(missingProjectsFields));
            }

            var missingContestantsFields =
                RequiredContestantsFields.Except(contestantsCsvReader.HeaderRecord).ToList();
            if (missingContestantsFields.Any())
            {
                throw new KnownException("Missing columns from projects csv file: " +
                                         JsonConvert.SerializeObject(missingContestantsFields));
            }

            await PrepareCache();
            return await Import(projectsCsvReader, contestantsCsvReader, debug);
        }

        public async Task<object> Import(CsvReader projectsCsvReader, CsvReader contestantsCsvReader, bool debug)
        {
            var projectsResult = new ImportResultModel();
            var participantsResult = new ImportResultModel();
            var participantsIdsToFind = new List<string>();
            var opResponse = await GetOldPlatformApiResponse();
            var cats = await _catsRepo.GetAll();

            while (await projectsCsvReader.ReadAsync())
            {
                var recordDn = projectsCsvReader.GetRecord<dynamic>() as IDictionary<string, object>;
                var recordDict = recordDn.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString());
                var opApiObj = opResponse.FirstOrDefault(p => p.Id == recordDict["Id"]);
                if (opApiObj == null)
                {
                    continue;
                }

                var project = ProjectFrom(recordDict, opApiObj, cats);
                var existingProject = GetProjectCaching(project.OldPlatformId);
                if (existingProject == null)
                {
                    if (!debug)
                        await AddProject(project);
                    else
                        _projectsCache.Add(project);

                    projectsResult.Added++;
                }
                else
                {
                    project.IsInOpen = existingProject.IsInOpen;
                    if (existingProject.Disabled)
                        existingProject.Disabled = false;

                    var patchDiff = JsonPatchUtils.CreatePatch(existingProject, project);
                    SanitizePatchDocument(patchDiff);
                    if (!patchDiff.IsEmpty())
                    {
                        if (!debug)
                            patchDiff.ApplyTo(existingProject);

                        projectsResult.Updated++;
                    }
                    else
                        projectsResult.NotTouched++;
                }

                var contestantsIds = recordDict["Id [Contestants]"];
                participantsIdsToFind.AddRange(contestantsIds.Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(id => id.Trim()));
                _seenProjectsIds.Add(project.OldPlatformId);
            }

            participantsIdsToFind = participantsIdsToFind.Distinct().ToList();

            while (await contestantsCsvReader.ReadAsync())
            {
                var recordEo = contestantsCsvReader.GetRecord<dynamic>() as IDictionary<string, object>;
                var recordDict = recordEo.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString());
                var opId = recordDict["Id"];
                if (!participantsIdsToFind.Contains(opId))
                {
                    continue;
                }

                var participant = ParticipantFrom(recordDict);
                var projectIds = recordDict["Id [Projects]"].Split(",", StringSplitOptions.RemoveEmptyEntries);
                var existingParticipant = GetParticipantCaching(participant.OldPlatformId);
                if (existingParticipant == null)
                {
                    if (!debug)
                    {
                        participant.User = await AddUserAsync(participant.User);
                        participant = await AddParticipant(participant);
                    }

                    participantsResult.Added++;
                }
                else
                {
                    participant.ActivationEmailSent = existingParticipant.ActivationEmailSent;
                    var patchDiff = JsonPatchUtils.CreatePatch(existingParticipant, participant);
                    SanitizePatchDocument(patchDiff);
                    if (!patchDiff.IsEmpty())
                    {
                        if (!debug)
                            patchDiff.ApplyTo(existingParticipant);

                        participantsResult.Updated++;
                    }
                    else
                        participantsResult.NotTouched++;

                    participant = existingParticipant;
                }

                foreach (var projectId in projectIds)
                {
                    participant.Projects ??= new();
                    if (participant.Projects.All(proj => proj.OldPlatformId != projectId))
                    {
                        var project = GetProjectCaching(projectId);
                        if (project == null)
                        {
                            participantsResult.Errors.Add(
                                $"required project with id {projectId} not found (for contestant with id: " + opId +
                                ")");
                        }
                        else
                        {
                            participantsResult.ProjectLinkAdded++;
                            participant.Projects.Add(project);
                        }
                    }
                }
            }

            var projectsNotSeen = _projectsCache.Select(p => p.OldPlatformId).Except(_seenProjectsIds).ToList();
            if (projectsNotSeen.Any())
            {
                projectsResult.Errors.AddRange(projectsNotSeen.Select(pid =>
                    $"Existing project not seen: {pid}. Set disabled = true for it."));
                foreach (var pid in projectsNotSeen)
                {
                    var project = GetProjectCaching(pid);
                    project.Disabled = true;
                }
            }

            return new { projectsResult, participantsResult };
        }

        public async Task<List<OldPlatformApiResponseModel>> GetOldPlatformApiResponse()
        {
            var httpClient = new HttpClient();
            var url = "https://api.infoeducatie.ro/v1/projects.json";
            var responseStr = await httpClient.GetStringAsync(url);
            var response = JsonConvert.DeserializeObject<List<OldPlatformApiResponseModel>>(responseStr);
            return response;
        }

        private async Task<User> AddUserAsync(User user)
        {
            var existing = await _userManager.FindByEmailAsync(user.Email);
            if (existing == null)
            {
                var userAddResult = await _userManager.CreateAsync(user);
                if (!userAddResult.Succeeded)
                {
                    throw new KnownException("Couldn't create user for: " + user.Email + " because " +
                                             string.Join(", ", userAddResult.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                user = existing;
            }


            if (!await _userManager.IsInRoleAsync(user, "Participant"))
            {
                var roleAddResult = await _userManager.AddToRoleAsync(user, "Participant");
                if (!roleAddResult.Succeeded)
                {
                    throw new KnownException("Couldn't add user in 'Participant' role: " + user.Email +
                                             " because " +
                                             string.Join(", ", roleAddResult.Errors.Select(e => e.Description)));
                }
            }

            return user;
        }

        private ProjectEntity ProjectFrom(IDictionary<string, string> record, OldPlatformApiResponseModel opApiModel,
            List<CategoryEntity> cats)
        {
            var project = new ProjectEntity
            {
                OldPlatformId = record["Id"],
                Title = record["Title"],
                Description = record["Description"],
                Technologies = record["Technical description"],
                SystemRequirements = record["System requirements"],
                SourceUrl = record["Source url"],
                Homepage = record["Homepage"],
                DiscourseUrl = opApiModel.Discourse_url,
                Category = cats.FirstOrDefault(c => c.Slug == opApiModel.Category)
            };
            return project;
        }

        private ParticipantEntity ParticipantFrom(IDictionary<string, string> record)
        {
            var participant = new ParticipantEntity
            {
                OldPlatformId = record["Id"],
                PhoneNumber = record["Phone number"],
                Grade = int.Parse(record["Grade"]),
                City = record["City"],
                County = record["County"],
                Country = record["Country"],
                School = record["School name"],
                SchoolCounty = record["School county"],
                SchoolCountry = record["School country"],
                SchoolCity = record["School city"],
                IdCardSeries = record["Id card type"],
                IdCardNumber = record["Id card number"],
                Cnp = record["Cnp"],
                MentoringTeacher = record["Mentoring teacher first name"] + " " + record["Mentoring teacher last name"],
                User = new User
                {
                    FirstName = record["First name [User]"],
                    LastName = record["Last name [User]"],
                    UserName = record["Email [User]"],
                    Email = record["Email [User]"]
                },
            };
            return participant;
        }

        private async Task<ProjectEntity> AddProject(ProjectEntity project)
        {
            _projectsCache.Add(project);
            return await _projectsRepo.Add(project);
        }

        private async Task<ParticipantEntity> AddParticipant(ParticipantEntity participant)
        {
            _participantsCache.Add(participant);
            return await _participantsRepo.Add(participant);
        }

        private ProjectEntity GetProjectCaching(string oldPlatformId)
        {
            return _projectsCache.FirstOrDefault(p => p.OldPlatformId == oldPlatformId);
        }

        private ParticipantEntity GetParticipantCaching(string oldPlatformId)
        {
            return _participantsCache.FirstOrDefault(p => p.OldPlatformId == oldPlatformId);
        }

        private void SanitizePatchDocument<T>(JsonPatchDocument<T> doc) where T : class
        {
            var forbiddenProps = new[]
            {
                "/Id", "/Created", "/Updated", "/User", "/Category", "/Projects", "/Participants",
                "/ProjectParticipants", "/$id", "/$ref"
            };
            foreach (var op in doc.Operations.ToList())
            {
                if (forbiddenProps.Any(fprop => op.path == fprop || op.path.StartsWith(fprop + "/")))
                {
                    doc.Operations.Remove(op);
                }
            }
        }

        private static readonly string[] RequiredProjectsFields =
        {
            "Title", "Description", "Technical description", "System requirements", "Source url", "Homepage",
            "Finished", "Open source", "Closed source reason", "Score", "Total score", "Id [Contestants]", "Id [Users]"
        };

        private static readonly string[] RequiredContestantsFields =
        {
            "Address", "City", "County", "Country", "Zip code", "Sex", "Phone number", "School name", "Grade",
            "School county", "School city", "School country", "Mentoring teacher first name",
            "Mentoring teacher last name", "Email [User]", "First name [User]", "Last name [User]", "Id [Projects]",
            "Id card type", "Id card number", "Cnp"
        };
    }
}