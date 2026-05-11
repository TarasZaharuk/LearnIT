using System;

namespace LearnIT.Domain.Entities;

public class Consultation
{
    public int Id { get; set; }

    public DateTime Start { get; set; }

    public DateTime? End { get; set; }

    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    public int AttendeeId { get; set; }

    public User Attendee { get; set; } = null!;

    public int TutorId { get; set; }

    public Tutor Tutor { get; set; } = null!;
}
