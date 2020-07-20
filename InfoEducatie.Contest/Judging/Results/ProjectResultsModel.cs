namespace InfoEducatie.Contest.Judging.Results
{
    public class ProjectResultsModel
    {
        public string ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public int ProjectPoints { get; set; }
        public int OpenPoints { get; set; }
        public int TotalPoints => ProjectPoints + OpenPoints;
    }
}