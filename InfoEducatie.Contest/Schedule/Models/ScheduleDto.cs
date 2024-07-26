using System.Collections.Generic;

namespace InfoEducatie.Contest.Schedule.Models;

public class ScheduleDto
{
    public ScheduleConfigModel Config { get; set; }
    public List<ScheduleCategoryDto> Categories { get; set; }
}