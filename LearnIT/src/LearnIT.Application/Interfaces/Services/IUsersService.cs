﻿using LearnIT.Application.DTOs;
using Shared;

namespace LearnIT.Application.Interfaces.Services
{
    public interface IUsersService
    {
        Task<List<UserDTO>> GetAsync();

        Task<int> AddAsync(AddUserModel user);

        Task DeleteByIdAsync(int id);
    }
}
