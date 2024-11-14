﻿using LearnIT.Domain.Entities;

namespace LearnIT.Application.Models
{
    public class AddTutorSkillsModel
    {
        public int TutorId { get; set; }

        public IList<string> Skills { get; set; } = [];
    }
}
