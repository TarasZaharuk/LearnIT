using LearnIT.Application.DTOs;
using Shared;
using Shared.AddUserResponse;

namespace LearnIT.Application.Interfaces.Services
{
    public interface IUsersService
    {
        Task<List<UserDTO>> GetAsync();

        Task<UserDTO?> GetByIdAsync(int id);

        Task<AddUserResponse> AddAsync(AddUserModel user);

        Task DeleteByIdAsync(int id);

        Task<UserDTO?> GetUserByLoginAsync(UserLoginModel userLoginModel);

        Task<bool> IsEmailConfirmed(int id);

        Task<string?> GetEmailByIdAsync(int id);
    }
}
