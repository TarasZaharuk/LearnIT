using LearnIT.Domain.Entities;

namespace LearnIT.Application.Models
{
    public class AddUserModel
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public Gender Gender { get; set; } = null!;

        public DateOnly BirthDate { get; set; }

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;
    }
}
