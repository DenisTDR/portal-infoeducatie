using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using InfoEducatie.Contest.Categories;
using InfoEducatie.Contest.Participants.Project;
using InfoEducatie.Contest.Schedule.Models;
using MCMS.Base.Attributes;
using MCMS.Base.Data;
using MCMS.Base.Helpers;
using Microsoft.EntityFrameworkCore;
using MoreLinq;
using Newtonsoft.Json;

namespace InfoEducatie.Contest.Schedule;

[Service]
public class ScheduleService(IRepository<CategoryEntity> catsRepo, IRepository<ProjectEntity> projectsRepo)
{
    public async Task<ScheduleDto> Generate(ScheduleConfigModel config)
    {
        projectsRepo.ChainQueryable(q => q.Where(p => !p.Disabled).Include(p => p.Participants));
        var categories = await catsRepo.Query.OrderBy(c => c.Name).ToListAsync();
        var dto = new ScheduleDto
        {
            Categories = [],
            Config = config,
        };
        foreach (var category in categories)
        {
            var catSchedule = await Generate(category, config);
            dto.Categories.Add(catSchedule);
        }

        return dto;
    }

    private async Task<ScheduleCategoryDto> Generate(CategoryEntity category, ScheduleConfigModel config)
    {
        var dto = new ScheduleCategoryDto
        {
            CategoryName = category.Name,
            Slots = []
        };
        var projects = await projectsRepo
            .GetAll(p => p.Category == category);
        projects = OrderBy(projects, config.OrderBy);

        var startTime = config.StartTime;
        var cnt = 1;

        foreach (var project in projects)
        {
            if (startTime >= config.PauseStartTime &&
                startTime < config.PauseStartTime.AddMinutes(config.PauseDuration))
            {
                dto.Slots.Add(new ScheduleSlotDto
                {
                    StartTime = config.PauseStartTime,
                    EndTime = config.PauseStartTime.AddMinutes(config.PauseDuration),
                    Title = "Pauză",
                    IsPause = true
                });
                startTime = config.PauseStartTime.AddMinutes(config.PauseDuration);
            }

            dto.Slots.Add(new ScheduleSlotDto
            {
                Title = project.Title, StartTime = startTime,
                EndTime = startTime.AddMinutes(category.PresentationSlotDuration),
                Hashtag = cnt.ToString()
            });

            startTime = startTime.AddMinutes(category.PresentationSlotDuration);
            cnt++;
        }


        return dto;
    }

    private List<ProjectEntity> OrderBy(List<ProjectEntity> projects, OrderBy orderBy)
    {
        switch (orderBy)
        {
            case Models.OrderBy.Random:
                projects = projects.Shuffle().ToList();
                break;
            case Models.OrderBy.OldPlatformId:
                projects = projects.OrderBy(p => p.OldPlatformId).ToList();
                break;
            case Models.OrderBy.Name:
                projects = projects.OrderBy(p => p.Title).ToList();
                break;
            case Models.OrderBy.CountyAndName:
                projects = projects
                    .OrderBy(p => p.Participants.First().County)
                    .ThenBy(p => p.Title)
                    .ToList();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(orderBy), orderBy, null);
        }


        return projects;
    }

    private static string Path => System.IO.Path.Join(Env.Get("CONTENT_PATH"), "schedule-config.json");

    public static async Task PersistConfig(ScheduleConfigModel config)
    {
        await using var sw = new StreamWriter(Path);
        await sw.WriteLineAsync(JsonConvert.SerializeObject(config, Formatting.Indented));
    }

    public static async Task<ScheduleConfigModel> GetConfig()
    {
        using var sr = new StreamReader(Path);
        var str = await sr.ReadToEndAsync();
        return JsonConvert.DeserializeObject<ScheduleConfigModel>(str);
    }
}