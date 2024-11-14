namespace Shared
{
    public class TutorsFilterModel
    {
        public string? Name { get; set;} = null!;

        public IList<string>? SelectedSkills { get; set; } = null!;

        public double? LowerWage { get; set; } = null!;

        public double? UpperWage { get; set; } = null!;

    }
}
