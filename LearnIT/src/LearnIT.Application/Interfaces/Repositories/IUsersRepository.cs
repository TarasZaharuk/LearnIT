﻿using LearnIT.Domain.Entities;

namespace LearnIT.Application.Interfaces.Repositories
{
    public interface IUsersRepository
    {
        Task<int> AddAsync(User user);

        Task AddAsync(List<User> users);

        Task DeleteByIdAsync(int id);

        Task<List<User>> GetAllAsync();
    }
}
