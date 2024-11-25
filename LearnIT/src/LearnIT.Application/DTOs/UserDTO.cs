namespace LearnIT.Application.DTOs
{
    public class UserDTO
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Gender {  get; set; } = null!; 

        public DateOnly BirthDate { get; set; }

        public int? TutorId { get; set; }
    }
}
