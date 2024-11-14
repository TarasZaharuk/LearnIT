namespace LearnIT.Domain.Entities
{
    public class TutorSkill
    {
        public TutorSkill(string skillName) 
        {
            SkillName = skillName;
            Id = 0;
        }
        public int Id { get; set; }

        public string SkillName { get; set; } = null!;

        public int TutorId { get; set; }
    }
}
