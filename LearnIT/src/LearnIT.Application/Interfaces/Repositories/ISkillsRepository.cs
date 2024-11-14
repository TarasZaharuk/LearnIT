using LearnIT.Domain.Entities;

namespace LearnIT.Application.Interfaces.Repositories
{
    public interface ISkillsRepository
    {
        Task AddAsync(GeneralSkill skill);

        Task AddAsync(List<GeneralSkill> skills);

        Task DeleteByIdAsync(int id);

        Task<List<GeneralSkill>> GetAllAsync();

        Task<List<GeneralSkill>> GetByIdsAsync(List<int> ids);
    }
}
