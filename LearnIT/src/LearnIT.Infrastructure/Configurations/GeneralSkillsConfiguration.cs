using LearnIT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnIT.Infrastructure.Configurations
{
    internal class GeneralSkillsConfiguration : IEntityTypeConfiguration<GeneralSkill>
    {
        public void Configure(EntityTypeBuilder<GeneralSkill> skillsBuilder)
        {
            skillsBuilder.HasData(
                new GeneralSkill { Id = 1, SkillName = ".Net" },
                new GeneralSkill { Id = 2, SkillName = "Blazor" },
                new GeneralSkill { Id = 3, SkillName = "HTML/CSS" },
                new GeneralSkill { Id = 4, SkillName = "JavaScript" },
                new GeneralSkill { Id = 5, SkillName = "TypeScript" },
                new GeneralSkill { Id = 6, SkillName = "React" },
                new GeneralSkill { Id = 7, SkillName = "Angular" },
                new GeneralSkill { Id = 8, SkillName = "Vue" },
                new GeneralSkill { Id = 9, SkillName = "C#" },
                new GeneralSkill { Id = 10, SkillName = "ASP.NET Core" },
                new GeneralSkill { Id = 11, SkillName = "Entity Framework" },
                new GeneralSkill { Id = 12, SkillName = "SQL" },
                new GeneralSkill { Id = 13, SkillName = "PostgreSQL" },
                new GeneralSkill { Id = 14, SkillName = "MongoDB" },
                new GeneralSkill { Id = 15, SkillName = "Docker" },
                new GeneralSkill { Id = 16, SkillName = "Kubernetes" },
                new GeneralSkill { Id = 17, SkillName = "Azure" },
                new GeneralSkill { Id = 18, SkillName = "AWS" },
                new GeneralSkill { Id = 19, SkillName = "Git" },
                new GeneralSkill { Id = 20, SkillName = "REST API" }
            );
        }
    }
}
