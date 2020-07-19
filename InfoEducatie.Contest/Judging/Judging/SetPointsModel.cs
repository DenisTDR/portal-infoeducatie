using System.ComponentModel.DataAnnotations;

namespace InfoEducatie.Contest.Judging.Judging
{
    public class SetPointsModel
    {
        [Required] public string CriterionId { get; set; }
        [Required] public string ProjectId { get; set; }
        [Required] public int Points { get; set; }
    }
}