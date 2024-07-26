using System;
using InfoEducatie.Base;
using Newtonsoft.Json;

namespace InfoEducatie.Contest.Schedule.Models;

public class ScheduleConfigModel
{
    public OrderBy OrderBy { get; set; }
    public string FirstDayTitle { get; set; } //Marți, 30 Iulie 2024

    [JsonConverter(typeof(TimeOnlyConverter))]
    public TimeOnly StartTime { get; set; }

    [JsonConverter(typeof(TimeOnlyConverter))]
    public TimeOnly PauseStartTime { get; set; }

    public int PauseDuration { get; set; } // minutes
}

public enum OrderBy
{
    Random,
    OldPlatformId,
    Name,
    CountyAndName,
}