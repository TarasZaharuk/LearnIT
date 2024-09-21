using LearnIT.Domain.Entities;

namespace LearnIT.Application.Models
{
    public class AddTutorModel
    {
        public int UserId { get; set; }

        public List<int> SkillsIds { get; set; } = [];

        public int? Rating { get; set; }

        public int WagePerHour { get; set; }

        public string JobTitle { get; set; } = null!;

        public string SummaryOfQualification { get; set; } = null!;

        public string? GitHubUrl { get; set; }

        public string? LinkedInUrl { get; set; }
    }
}
