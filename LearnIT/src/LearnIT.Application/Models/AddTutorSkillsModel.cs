namespace LearnIT.Application.Models
{
    public class AddTutorSkillsModel
    {
        public int TutorId { get; set; }

        public List<int> SkillIds { get; set; } = [];
    }
}
