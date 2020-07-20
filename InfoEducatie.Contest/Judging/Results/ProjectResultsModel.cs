namespace InfoEducatie.Contest.Judging.Results
{
    public class ProjectResultsModel
    {
        public string ProjectId { get; set; }
        public string ProjectTitle { get; set; }
        public int TotalProjectPoints { get; set; }
        public int TotalOpenPoints { get; set; }
        public int TotalPoints => TotalProjectPoints + TotalOpenPoints;

        public float GetProjectPoints(int judges)
        {
            if (judges < 2)
            {
                return TotalProjectPoints;
            }

            return 1.0f * TotalProjectPoints / judges;
        }

        public float GetOpenPoints(int judges)
        {
            if (judges < 2)
            {
                return TotalOpenPoints;
            }

            return 1.0f * TotalOpenPoints / judges;
        }

        public float GetFinalPoints(int projectJudges, int openJudges)
        {
            return GetProjectPoints(projectJudges) + GetOpenPoints(openJudges);
        }
    }
}