using System;
using System.Collections.Generic;
using InfoEducatie.Base;
using Newtonsoft.Json;

namespace InfoEducatie.Contest.Schedule.Models;

public class ScheduleConfigModel
{
    public OrderBy OrderBy { get; set; }
    public List<ScheduleDayConfigModel> Days { get; set; }
}

public class ScheduleDayConfigModel
{
    public string Title { get; set; } //Marți, 30 Iulie 2024

    [JsonConverter(typeof(TimeOnlyConverter))]
    public TimeOnly StartTime { get; set; }

    [JsonConverter(typeof(TimeOnlyConverter))]
    public TimeOnly PauseStartTime { get; set; }

    public int PauseDuration { get; set; } // minutes

    [JsonConverter(typeof(TimeOnlyConverter))]
    public TimeOnly EndTime { get; set; }

    public ScheduleCategoryDayBagDto ToBag()
    {
        return new ScheduleCategoryDayBagDto()
        {
            Title = Title,
            Slots = [],
        };
    }
}

public enum OrderBy
{
    FixedRandom,
    Random,
    OldPlatformId,
    Name,
    CountyAndName,
}