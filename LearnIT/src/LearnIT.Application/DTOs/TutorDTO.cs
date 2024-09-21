using LearnIT.Domain.Entities;

namespace LearnIT.Application.DTOs
{
    public class TutorDTO
    {
        public int Id { get; set; }

        public UserDTO User { get; set; } = null!;

        public List<Skill> Skills { get; set; } = [];

        public int? Rating { get; set; }  

        public int WagePerHour { get; set; }

        public string JobTitle { get; set; } = null!;

        public string SummaryOfQualification { get; set; } = null!;

        public string? GitHubUrl { get; set; }

        public string? LinkedInUrl { get; set; }
    }
}
