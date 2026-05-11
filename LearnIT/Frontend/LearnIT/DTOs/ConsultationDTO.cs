using System;
using Heron.MudCalendar;

namespace LearnIT.DTOs;

    public class ConsultationDTO : CalendarItem
    {
        public string Title { get; set; } = string.Empty;

        public string? Description { get; set; }

        public int AttendeeId { get; set; }

        public UserDTO? Attendee { get; set; }

        public int TutorId { get; set; }

        public TutorDTO? Tutor { get; set; }
    }
