﻿using Shared;
using LearnIT.Domain.Entities;

namespace LearnIT.Application.Interfaces.Repositories
{
    public interface ITutorsRepository
    {
        Task<int> AddAsync(Tutor tutor);

        Task AddAsync(List<Tutor> tutors);

        Task DeleteAsync(int id);

        Task DeleteAllAsync();

        Task<List<Tutor>> GetAllAsync();

        Task<List<Tutor>> GetAsync(TutorsFilterModel filter, EntityState entityState);

        Task<Tutor?> GetByIdAsync(int id);

        Task UpdateAsync(Tutor updatedTutor);
    }
}
