using LearnIT.Domain.Entities;

namespace LearnIT.Application.Interfaces.Repositories
{
    public interface IGendersRepository
    {
        Task AddAsync(Gender gender);

        Task AddGendersAsync(List<Gender> genders);

        Task DeleteAsync(Gender gender);

        Task<List<Gender>> GetAllAsync();
    }
}
