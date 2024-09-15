using LearnIT.Domain.Entities;

namespace LearnIT.Application.Interfaces.Repositories
{
    public interface IUsersRepository
    {
        Task AddAsync(User user);

        Task AddUsersAsync(List<User> users);

        Task DeleteAsync(User user);

        Task<List<User>> GetAllAsync();
    }
}
