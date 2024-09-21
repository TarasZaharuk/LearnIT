using LearnIT.Domain.Entities;
using LearnIT.Infrastructure.Configurations;
using Microsoft.EntityFrameworkCore;


namespace LearnIT.Infrastructure.Persistence
{
    public class LearnITDBContext : DbContext
    {
        public LearnITDBContext(DbContextOptions<LearnITDBContext> options) : base(options) 
        { 

        }

        public DbSet<User> Users { get; set; }

        public DbSet<Gender> Genders { get; set; }

        public DbSet<Skill> Skills { get; set; }

        public DbSet<Tutor> Tutors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UsersConfiguration());
            modelBuilder.ApplyConfiguration(new GendersConfiguration());
            modelBuilder.ApplyConfiguration(new TutorsConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}  
