using LearnIT.Application.Interfaces.Repositories;
using LearnIT.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearnIT.Infrastructure.Persistence.Repositories
{
    public class TutorsRepository : ITutorsRepository
    {
        private readonly LearnITDBContext _learnITDBContext;
        public TutorsRepository(LearnITDBContext learnITDBContext)
        {
            _learnITDBContext = learnITDBContext;
        }
        public async Task AddAsync(Tutor tutor)
        {
            await _learnITDBContext.Tutors.AddAsync(tutor);
            await _learnITDBContext.SaveChangesAsync();
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
