using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CsvHelper;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Participants.Participant;
using InfoEducatie.Contest.Participants.Project;
using MCMS.Base.Auth;
using MCMS.Base.Exceptions;
using MCMS.Base.JsonPatch;
using MCMS.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

namespace InfoEducatie.Main.InfoEducatieAdmin
{
    public class ImportService
    {
        private readonly IRepository<ProjectEntity> _projectsRepo;
        private readonly IRepository<ParticipantEntity> _participantsRepo;
        private readonly IRepository<CategoryEntity> _catsRepo;
        private readonly UserManager<User> _userManager;
        private readonly BaseDbContext _dbContext;
        private List<ProjectEntity> _projectsCache;
        private List<ParticipantEntity> _participantsCache;

        public ImportService(IRepository<ProjectEntity> projectsRepo, IRepository<ParticipantEntity> participantsRepo,
            UserManager<User> userManager, BaseDbContext dbContext, IRepository<CategoryEntity> catsRepo)
        {
            _projectsRepo = projectsRepo;
            _participantsRepo = participantsRepo;
            _userManager = userManager;
            _dbContext = dbContext;
            _catsRepo = catsRepo;
            _projectsRepo.ChainQueryable(q => q.Include(p => p.Category));
            _participantsRepo.ChainQueryable(q => q.Include(p => p.Project));
        }

        private async Task PrepareCache()
        {
            // _projectsRepo.SkipSaving = _participantsRepo.SkipSaving = true;
            _projectsCache = await _projectsRepo.GetAll();
            _participantsCache = await _participantsRepo.GetAll();
        }

        public async Task<ImportResultModel> ImportParticipantsCsv(string str, bool fixFuckedEncoding)
        {
            if (fixFuckedEncoding)
            {
                str = FuckedEncodingHelper.FixFuckedEncoding(str);
            }

            await PrepareCache();
            using var strReader = new StringReader(str);
            using var csvReader = new CsvReader(strReader, CultureInfo.InvariantCulture);
            var result = new ImportResultModel();
            var cats = await _catsRepo.GetAll();
            while (await csvReader.ReadAsync())
            {
                result.Rows++;
                ExpandoObject recordEo = csvReader.GetRecord<dynamic>();
                var recordDict = recordEo.ToDictionary(kvp => kvp.Key, kvp => kvp.Value?.ToString());
                result.Add(await ImportRow(recordDict, cats));
            }

            if (result.Added > 0 || result.Updated > 0)
            {
                await _dbContext.SaveChangesAsync();
            }

            return result;
        }

        private async Task<ImportResultModel> ImportRow(IDictionary<string, string> recordRow,
            List<CategoryEntity> cats)
        {
            var result = new ImportResultModel();
            var project = ProjectFrom(recordRow);
            project.Category = cats[new Random().Next(0, cats.Count)];

            if (string.IsNullOrEmpty(project.OldPlatformId?.Trim()))
            {
                result.ErrorCount++;
                result.Errors.Add("Participant without project at Id=" + recordRow["Id"]);
                return result;
            }

            var participant = ParticipantFrom(recordRow);

            var existingProject = GetProjectCaching(project.OldPlatformId);
            if (existingProject == null)
            {
                // existingProject = await _projectsRepo.Add(project);
                existingProject = await AddProject(project);
                result.Added++;
            }
            else
            {
                var patchDiff = JsonPatchUtils.CreatePatch(existingProject, project);
                SanitizePatchDocument(patchDiff);
                if (!patchDiff.IsEmpty())
                {
                    patchDiff.ApplyTo(existingProject);
                    result.Updated++;
                }
            }

            // var existingParticipant = await _participantsRepo.GetOne(p => p.OldPlatformId == participant.OldPlatformId);
            var existingParticipant = GetParticipantCaching(participant.OldPlatformId);
            if (existingParticipant == null)
            {
                participant.Project = existingProject;
                var userAddResult = await _userManager.CreateAsync(participant.User);
                if (!userAddResult.Succeeded)
                {
                    throw new KnownException("Couldn't create user for: " + participant.User.Email + " because " +
                                             string.Join(", ", userAddResult.Errors.Select(e => e.Description)));
                }

                var roleAddResult = await _userManager.AddToRoleAsync(participant.User, "Participant");
                if (!roleAddResult.Succeeded)
                {
                    throw new KnownException("Couldn't add user in participant role: " + participant.User.Email +
                                             " because " +
                                             string.Join(", ", roleAddResult.Errors.Select(e => e.Description)));
                }

                // existingParticipant = await _participantsRepo.Add(participant);
                existingParticipant = await AddParticipant(participant);
                result.Added++;
            }
            else
            {
                if (participant.Project != existingProject)
                {
                    participant.Project = existingProject;
                }

                var patchDiff = JsonPatchUtils.CreatePatch(existingParticipant, participant);
                SanitizePatchDocument(patchDiff);
                if (!patchDiff.IsEmpty())
                {
                    patchDiff.ApplyTo(existingParticipant);
                    result.Updated++;
                }
            }

            return result;
        }

        private ProjectEntity ProjectFrom(IDictionary<string, string> record)
        {
            var project = new ProjectEntity
            {
                OldPlatformId = record["Id [Projects]"],
                Title = record["Title [Projects]"],
                Description = record["Description [Projects]"],
                Technologies = record["Technical description [Projects]"],
                SystemRequirements = record["System requirements [Projects]"],
                SourceUrl = record["Source url [Projects]"],
                Homepage = record["Homepage [Projects]"],
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
            var forbiddenProps = new[] {"/Id", "/Created", "/Updated", "/User", "/Category"};
            foreach (var op in doc.Operations.ToList())
            {
                if (forbiddenProps.Any(fprop => op.path.StartsWith(fprop)))
                {
                    doc.Operations.Remove(op);
                }
            }
        }
    }
}