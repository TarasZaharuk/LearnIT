using System;
using Bogus;
using Microsoft.EntityFrameworkCore;
using LearnIT.Domain.Entities;
using LearnIT.Infrastructure.Persistence;

namespace LearnIT.Infrastructure;

public static class DBSeeder
{
    public static void Seed(LearnITDBContext dbContext)
    {
        var userFaker = new Faker<User>()
            .RuleFor(u => u.FirstName, f => f.Name.FirstName())
            .RuleFor(u => u.LastName, f => f.Name.LastName())
            .RuleFor(u => u.Email, (f, u) => f.Internet.Email(u.FirstName, u.LastName))
            .RuleFor(u => u.Password, f => f.Phone.PhoneNumber())
            .RuleFor(u => u.GenderId, f => f.Random.Number(1, 2));
    }
}
