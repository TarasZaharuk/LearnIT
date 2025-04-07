using LearnIT.Application.DTOs;
using Shared;

namespace LearnIT.Application.Interfaces.Services
{
    public interface IUsersService
    {
        Task<List<UserDTO>> GetAsync();

        Task<UserDTO?> GetByIdAsync(int id);

        Task<string> AddAsync(AddUserModel user);

        Task DeleteByIdAsync(int id);

        Task<UserDTO?> GetUserByLoginAsync(UserLoginModel userLoginModel);

        Task<bool> IsEmailConfirmed(int userId);

    }
}
