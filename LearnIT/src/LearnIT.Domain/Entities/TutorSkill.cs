namespace LearnIT.Domain.Entities
{
    public class TutorSkill
    {
        public int Id { get; set; }

        public string SkillName { get; set; } = null!;

        public int TutorId { get; set; }
    }
}
