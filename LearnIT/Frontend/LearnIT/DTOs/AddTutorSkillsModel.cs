namespace LearnIT.DTOs
{
    public class AddTutorSkillsModel
    {
        public int TutorId { get; set; }

        public IList<string> Skills { get; set; } = [];
    }
}
