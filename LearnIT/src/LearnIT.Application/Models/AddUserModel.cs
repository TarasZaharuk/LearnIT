using LearnIT.Domain.Entities;

namespace LearnIT.Application.Models
{
    public class AddUserModel
    {
        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public int GenderId { get; set; }

        public DateOnly BirthDate { get; set; }
    }
}
