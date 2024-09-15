using LearnIT.Application.DTOs;
using LearnIT.Application.Models;
using LearnIT.Domain.Entities;

namespace LearnIT.Application.Interfaces.Services
{
    public interface IUsersService
    {
        Task<List<UserDTO>> GetUsersAsync();

        Task AddUserAsync(AddUserModel user);

        Task DeleteUserAsync(User user);
    }
}
