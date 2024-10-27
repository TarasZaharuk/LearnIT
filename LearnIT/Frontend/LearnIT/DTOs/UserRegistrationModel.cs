namespace LearnIT.DTOs
{
    public class UserRegistrationModel
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Gender { get; set; } = null!;

        public DateOnly BirthDate { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
