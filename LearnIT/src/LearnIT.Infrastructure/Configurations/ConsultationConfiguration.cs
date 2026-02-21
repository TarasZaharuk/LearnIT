using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using LearnIT.Domain.Entities;

namespace LearnIT.Infrastructure.Configurations;

public class ConsultationConfiguration: IEntityTypeConfiguration<Consultation>
    {
        public void Configure(EntityTypeBuilder<Consultation> consultationBuilder)
        {
            consultationBuilder
                .Property(c => c.Title)
                .IsRequired()
                .HasMaxLength(255);
            consultationBuilder
                .Property(c => c.Description)
                .HasMaxLength(1000)
                .IsRequired(false);
            consultationBuilder.
                HasOne(c => c.Attendee)
                .WithMany()
                .HasForeignKey(c => c.AttendeeId)
                .OnDelete(DeleteBehavior.NoAction);
            consultationBuilder.
                HasOne(c => c.Tutor)
                .WithMany()
                .HasForeignKey(c => c.TutorId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
