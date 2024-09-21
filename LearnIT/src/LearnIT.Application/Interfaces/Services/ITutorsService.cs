using LearnIT.Application.DTOs;
using LearnIT.Application.Models;
using LearnIT.Domain.Entities;

namespace LearnIT.Application.Interfaces.Services
{
    public interface ITutorsService
    {
        Task<List<TutorDTO>> GetAsync();

        Task AddAsync(AddTutorModel tutor);

        Task DeleteByIdAsync(int id);

        Task DeleteAllAsync();

        Task AddSkillsAsync(AddTutorSkillsModel addTutorSkills);
    }
}
