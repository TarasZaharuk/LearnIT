namespace LearnIT.Domain.Entities
{
    public class User
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public Gender Gender { get; set; } = null!;

        public int GenderId { get; set; }

        public DateOnly BirthDay { get; set; }
    }
}
