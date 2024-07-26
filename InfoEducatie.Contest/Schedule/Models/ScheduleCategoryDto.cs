using System.Collections.Generic;

namespace InfoEducatie.Contest.Schedule.Models;

public class ScheduleCategoryDto
{
    public string CategoryName { get; set; }
    public List<ScheduleCategoryDayBagDto> Days { get; set; }
}

public class ScheduleCategoryDayBagDto
{
    public string Title { get; set; }
    public List<ScheduleSlotDto> Slots { get; set; }
}