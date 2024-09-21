using LearnIT.Domain.Entities;

namespace LearnIT.Application.Interfaces.Repositories
{
    public interface ISkillsRepository
    {
        Task AddAsync(Skill skill);

        Task AddAsync(List<Skill> skills);

        Task DeleteByIdAsync(int id);

        Task<List<Skill>> GetAllAsync();

        Task<List<Skill>> GetByIdsAsync(List<int> ids);
    }
}
