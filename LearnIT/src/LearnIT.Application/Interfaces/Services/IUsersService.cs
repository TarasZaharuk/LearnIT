using LearnIT.Application.DTOs;
using LearnIT.Application.Models;
using LearnIT.Domain.Entities;

namespace LearnIT.Application.Interfaces.Services
{
    public interface IUsersService
    {
        Task<List<UserDTO>> GetAsync();

        Task AddAsync(AddUserModel user);

        Task DeleteByIdAsync(int id);
    }
}
