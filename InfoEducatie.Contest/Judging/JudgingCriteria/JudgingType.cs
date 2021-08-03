namespace InfoEducatie.Contest.Judging.JudgingCriteria
{
    public enum JudgingType
    {
        Project,
        Open
    }

    public enum JudgeType
    {
        Project,
        Open,
        Both,
        None,
    }

    public static class JudgeTypeExtensions
    {
        public static bool JudgesProject(this JudgeType type)
        {
            return type is JudgeType.Both or JudgeType.Project;
        }

        public static bool JudgesOpen(this JudgeType type)
        {
            return type is JudgeType.Both or JudgeType.Open;
        }
    }
}