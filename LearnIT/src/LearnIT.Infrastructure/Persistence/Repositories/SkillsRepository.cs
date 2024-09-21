using LearnIT.Application.Interfaces.Repositories;
using LearnIT.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearnIT.Infrastructure.Persistence.Repositories
{
    public class SkillsRepository : ISkillsRepository
    {
        private readonly LearnITDBContext _learnITDBcontext;
        public SkillsRepository(LearnITDBContext learnITDBContext)
        {
            _learnITDBcontext = learnITDBContext;
        }
        public async Task AddAsync(Skill skill)
        {
            await _learnITDBcontext.AddAsync(skill);
            await _learnITDBcontext.SaveChangesAsync();
        }

        public async Task AddAsync(List<Skill> skills)
        {
            await _learnITDBcontext.AddRangeAsync(skills);
            await _learnITDBcontext.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            Skill? skill = await _learnITDBcontext.Skills.FindAsync(id);
            if (skill == null)
                return;

            _learnITDBcontext.Skills.Remove(skill);
            await _learnITDBcontext.SaveChangesAsync();
        }

        public async Task<List<Skill>> GetAllAsync()
        {
            return await _learnITDBcontext.Skills.ToListAsync();
        }

        public async Task<List<Skill>> GetByIdsAsync(List<int> ids)
        {
            return await _learnITDBcontext.Skills.Where(skill => ids.Contains(skill.Id)).ToListAsync();
        }
    }
}
