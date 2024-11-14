using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnIT.Application.Models
{
    public class UpdateTutorGeneralInfoModel
    {
        public int TutorId { get; set; }

        public int? WagePerHour { get; set; }

        public string? JobTitle { get; set; } = null!;

        public string? SummaryOfQualification { get; set; } = null!;

        public string? GitHubUrl { get; set; }

        public string? LinkedInUrl { get; set; }
    }
}
