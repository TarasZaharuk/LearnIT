using LearnIT.Application.Interfaces.Repositories;
using LearnIT.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Shared;

namespace LearnIT.Infrastructure.Persistence.Repositories
{
    public class TutorsRepository : ITutorsRepository
    {
        private readonly LearnITDBContext _learnITDBContext;
        public TutorsRepository(LearnITDBContext learnITDBContext)
        {
            _learnITDBContext = learnITDBContext;
        }
        public async Task<int> AddAsync(Tutor tutor)
        {
            await _learnITDBContext.Tutors.AddAsync(tutor);
            await _learnITDBContext.SaveChangesAsync();
            return tutor.Id;
        }

        public async Task AddAsync(List<Tutor> tutors)
        {
            await _learnITDBContext.AddRangeAsync(tutors);
            await _learnITDBContext.SaveChangesAsync();
        }

        public async Task DeleteAllAsync()
        {
            await _learnITDBContext.Tutors.ExecuteDeleteAsync();
            await _learnITDBContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            Tutor? tutor = await _learnITDBContext.Tutors.FindAsync(id);
            if (tutor == null)
                return;

            _learnITDBContext.Tutors.Remove(tutor);
            await _learnITDBContext.SaveChangesAsync();
        }

        public async Task<List<Tutor>> GetAllAsync()
        {
            return await _learnITDBContext.Tutors
               .Include(t => t.User)
               .Include(t => t.User.Gender)
               .Include(t => t.Skills)
               .ToListAsync();
        }

        public async Task<List<Tutor>> GetActiveAsync(TutorsFilterModel filter)
        {
            IQueryable<Tutor> filteredTutors = _learnITDBContext.Tutors
                .Include(t => t.User)
                .Include(t => t.User.Gender)
                .Include(t => t.Skills)
                .Where(t => t.EntityState == Domain.Entities.EntityState.Active);
            if (filter == null)
                return await filteredTutors.ToListAsync();

            if (!string.IsNullOrEmpty(filter.Name))
                filteredTutors = filteredTutors.Where(t => t.User.FirstName.Contains(filter.Name) || t.User.LastName.Contains(filter.Name));
            if (filter.SelectedSkills != null && filter.SelectedSkills.Any())
                filteredTutors = filteredTutors.Where(t => filter.SelectedSkills.All(sk => t.Skills.Select(t => t.SkillName).Contains(sk)));
            if (filter.LowerWage != null)
                filteredTutors = filteredTutors.Where(t => t.WagePerHour >= filter.LowerWage);
            if (filter.UpperWage != null)
                filteredTutors = filteredTutors.Where(t => t.WagePerHour <= filter.UpperWage);

            return await filteredTutors.ToListAsync();
        }

        public async Task<Tutor?> GetByIdAsync(int id)
        {
            return await _learnITDBContext.Tutors
                .Include(t => t.User)
                .Include(t => t.User.Gender)
                .Include(t => t.Skills)
                .SingleOrDefaultAsync(t => t.Id == id);
        }

        public async Task UpdateAsync(Tutor updatedTutor)
        {
            _learnITDBContext.Tutors.Update(updatedTutor);
            await _learnITDBContext.SaveChangesAsync();
        }
    }
}
