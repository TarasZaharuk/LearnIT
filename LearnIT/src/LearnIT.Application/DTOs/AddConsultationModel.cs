namespace LearnIT.Application.DTOs
{
    public class AddConsultationModel
    {
        public DateTime Start { get; set; }

        public DateTime? End { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int AttendeeId { get; set; }

        public int TutorId { get; set; }
    }
}
