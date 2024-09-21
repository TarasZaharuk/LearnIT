using LearnIT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LearnIT.Infrastructure.Configurations
{
    public class TutorsConfiguration : IEntityTypeConfiguration<Tutor>
    {
        public void Configure(EntityTypeBuilder<Tutor> tutorsBuilder)
        {
            tutorsBuilder
                .Property(t => t.Rating)
                .IsRequired(false);
            tutorsBuilder.
                HasOne(t => t.User)
                .WithOne()
                .HasForeignKey<Tutor>(t => t.UserId);
            tutorsBuilder.
                HasMany(t => t.Skills)
                .WithMany();
        }
    }
}
