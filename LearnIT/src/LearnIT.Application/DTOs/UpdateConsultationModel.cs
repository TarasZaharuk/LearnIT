namespace LearnIT.Application.DTOs
{
    public class UpdateConsultationModel
    {
        public int Id { get; set; }

        public DateTime Start { get; set; }

        public DateTime? End { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int AttendeeId { get; set; }

        public int TutorId { get; set; }
    }
}
