namespace Shared
{
    public class AddUserModel
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public int GenderId { get; set; }

        public DateOnly BirthDate { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
