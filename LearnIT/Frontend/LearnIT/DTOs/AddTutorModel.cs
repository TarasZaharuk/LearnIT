﻿using Shared;

namespace LearnIT.DTOs
{
    public class AddTutorModel
    {
        public int UserId { get; set; }

        public IList<string> Skills { get; set; } = [];

        public int? Rating { get; set; }

        public int? WagePerHour { get; set; }

        public string? JobTitle { get; set; }

        public string? SummaryOfQualification { get; set; }

        public string? GitHubUrl { get; set; }

        public string? LinkedInUrl { get; set; }
    }
}
