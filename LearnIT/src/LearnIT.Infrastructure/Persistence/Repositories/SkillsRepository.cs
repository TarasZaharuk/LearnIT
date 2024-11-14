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
        public async Task AddAsync(GeneralSkill skill)
        {
            await _learnITDBcontext.AddAsync(skill);
            await _learnITDBcontext.SaveChangesAsync();
        }

        public async Task AddAsync(List<GeneralSkill> skills)
        {
            await _learnITDBcontext.AddRangeAsync(skills);
            await _learnITDBcontext.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            GeneralSkill? skill = await _learnITDBcontext.GeneralSkills.FindAsync(id);
            if (skill == null)
                return;

            _learnITDBcontext.GeneralSkills.Remove(skill);
            await _learnITDBcontext.SaveChangesAsync();
        }

        public async Task<List<GeneralSkill>> GetAllAsync()
        {
            return await _learnITDBcontext.GeneralSkills.ToListAsync();
        }

        public async Task<List<GeneralSkill>> GetByIdsAsync(List<int> ids)
        {
            return await _learnITDBcontext.GeneralSkills.Where(skill => ids.Contains(skill.Id)).ToListAsync();
        }
    }
}
