using System;
using InfoEducatie.Base;
using Newtonsoft.Json;

namespace InfoEducatie.Contest.Schedule.Models;

public class ScheduleSlotDto
{
    public string Title { get; set; }

    [JsonConverter(typeof(TimeOnlyConverter))]
    public TimeOnly StartTime { get; set; }

    [JsonConverter(typeof(TimeOnlyConverter))]
    public TimeOnly EndTime { get; set; }

    public bool IsBreak { get; set; }
    public string Hashtag { get; set; }
}