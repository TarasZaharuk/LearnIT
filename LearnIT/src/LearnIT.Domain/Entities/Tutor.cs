namespace LearnIT.Domain.Entities
{
    public class Tutor
    {
        public int Id { get; set; }

        public User User { get; set; } = null!;

        public int UserId { get; set; }

        public ICollection<Skill> Skills { get; set; } = [];

        public int? Rating { get; set; }

        public int WagePerHour { get; set; }

        public string JobTitle { get; set; } = null!;

        public string SummaryOfQualification { get; set; } = null!;

        public string? GitHubUrl { get; set; }

        public string? LinkedInUrl { get; set; }

        public byte[]? Logo { get; set; } = null!;
    }
}
