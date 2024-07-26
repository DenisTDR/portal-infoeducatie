using System;
using System.Collections.Generic;

namespace InfoEducatie.Contest.Schedule.Models;

public class ScheduleCategoryDto
{
    public string CategoryName { get; set; }
    public List<ScheduleSlotDto> Slots { get; set; }
}