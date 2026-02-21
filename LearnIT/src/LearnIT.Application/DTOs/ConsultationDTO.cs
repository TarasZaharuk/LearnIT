namespace LearnIT.Application.DTOs
{
    public class ConsultationDTO
    {
        public int Id { get; set; }

        public DateTime Start { get; set; }

        public DateTime? End { get; set; }

        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int AttendeeId { get; set; }

        public UserDTO? Attendee { get; set; }

        public int TutorId { get; set; }

        public TutorDTO? Tutor { get; set; }
    }
}
