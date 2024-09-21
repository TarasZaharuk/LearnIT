using LearnIT.Application.Interfaces.Repositories;
using LearnIT.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearnIT.Infrastructure.Persistence.Repositories
{
    public class GendersRepository : IGendersRepository
    {
        private LearnITDBContext _learnItDbContext;
        public GendersRepository(LearnITDBContext learnItDbContext)
        {
            _learnItDbContext = learnItDbContext;
        }
        public async Task AddAsync(Gender gender)
        {
            _learnItDbContext.Add(gender);
            await _learnItDbContext.SaveChangesAsync();
        }

        public async Task AddGendersAsync(List<Gender> genders)
        {
            await _learnItDbContext.AddRangeAsync(genders);
            await _learnItDbContext.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            Gender? gender = await _learnItDbContext.Genders.FindAsync(id);
            if (gender == null)
                return;

            _learnItDbContext.Genders.Remove(gender);
            await _learnItDbContext.SaveChangesAsync();
        }

        public async Task<List<Gender>> GetAllAsync()
        {
            return await _learnItDbContext.Genders.ToListAsync();
        }

    }
}
