using Bogus;
using LearnIT.Domain.Entities;
using LearnIT.Infrastructure.Persistence;

namespace LearnIT.Infrastructure;

public class DBSeeder(LearnITDBContext dbContext)
{
    private static readonly string[] JobTitles =
    [
        "Senior .Net Developer",
        "Junior .Net Developer",
        "Frontend Developer",
        "Fullstack Developer",
        "Software Architect"
    ];

    private static readonly int[] ConsultationDurationsInMinutes = [30, 60, 90, 120];

    private const int UsersCount = 20;
    private const int TutorsCount = 15;
    private const int ConsultationsCount = 50;

    private List<User> _users = [];
    private List<Tutor> _tutors = [];

    public void Seed()
    {
        SeedUsers();
        SeedTutors();
        SeedConsultations();
        dbContext.SaveChanges();
    }

    private void SeedUsers()
    {
        if (dbContext.Users.Any())
            return;

        var userFaker = new Faker<User>()
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
            .RuleFor(u => u.EmailConfirmed, f => f.Random.Bool())
            .RuleFor(u => u.Password, f => f.Phone.PhoneNumber())
            .RuleFor(u => u.GenderId, f => f.Random.Number(1, 2))
            .RuleFor(u => u.BirthDate, f => DateOnly.FromDateTime(f.Date.Past(30, DateTime.Now.AddYears(-18))));

        _users = userFaker.Generate(UsersCount).ToList();
        dbContext.Users.AddRange(_users);
    }

    private void SeedTutors()
    {
        if (dbContext.Tutors.Any())
            return;

        var userQueue = new Queue<User>(_users.OrderBy(_ => Guid.NewGuid()));
        var skillNames = dbContext.GeneralSkills.Select(s => s.SkillName).ToList();
        var tutorFaker = new Faker<Tutor>()
            .RuleFor(t => t.User, _ => userQueue.Dequeue())
            .RuleFor(t => t.SummaryOfQualification, f => f.Lorem.Paragraph())
            .RuleFor(t => t.Rating, f => f.Random.Number(1, 5))
            .RuleFor(t => t.LinkedInUrl, f => "https://www.linkedin.com/in/" + f.Internet.UserName())
            .RuleFor(t => t.GitHubUrl, f => "https://github.com/" + f.Internet.UserName())
            .RuleFor(t => t.JobTitle, f => f.PickRandom(JobTitles))
            .RuleFor(t => t.Skills, f => skillNames.OrderBy(_ => f.Random.Number()).Take(f.Random.Number(3, 7)).Select(s => new TutorSkill(s)).ToList())
            .RuleFor(t => t.WagePerHour, f => f.Random.Number(10, 100))
            .RuleFor(t => t.EntityState, _ => EntityState.Active);

        _tutors = tutorFaker.Generate(TutorsCount).ToList();
        dbContext.Tutors.AddRange(_tutors);
    }

    private void SeedConsultations()
    {
        if (dbContext.Consultations.Any())
            return;

        var consultationFaker = new Faker<Consultation>()
            .RuleFor(c => c.Tutor, f => f.PickRandom(_tutors))
            .RuleFor(c => c.Attendee, (f, c) => f.PickRandom(_users.Where(u => u != c.Tutor.User).ToList()))
            .RuleFor(c => c.Start, f => f.Date.Between(DateTime.Now.AddDays(-30), DateTime.Now.AddDays(30)))
            .RuleFor(c => c.End, (f, c) => c.Start.AddMinutes(f.PickRandom(ConsultationDurationsInMinutes)))
            .RuleFor(c => c.Title, (f, c) => $"Consultation with {c.Tutor.User.FirstName} {c.Tutor.User.LastName}")
            .RuleFor(c => c.Description, f => f.Random.Bool() ? f.Lorem.Paragraph() : null);

        var consultations = consultationFaker.Generate(ConsultationsCount);
        dbContext.Consultations.AddRange(consultations);
    }
}
