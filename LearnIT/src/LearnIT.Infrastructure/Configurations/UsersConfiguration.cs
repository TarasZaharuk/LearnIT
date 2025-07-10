using LearnIT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnIT.Infrastructure.Configurations
{
    internal class UsersConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> usersBuilder)
        {
            usersBuilder
                .HasOne(u => u.Gender)
                .WithMany()
                .HasForeignKey(u => u.GenderId);
            usersBuilder.HasIndex(u => u.Email).IsUnique();
        }
    }
}
