using LearnIT.Application.DTOs;
using LearnIT.Application.Models;

namespace LearnIT.Application.Interfaces.Services
{
    public interface ITutorsService
    {
        Task<List<TutorDTO>> GetAsync();

        Task AddAsync(AddTutorModel tutor);

        Task DeleteByIdAsync(int id);

        Task DeleteAllAsync();

        Task AddSkillsAsync(AddTutorSkillsModel addTutorSkills);

        Task SetLogoAsync(int tutorId , byte[] logo);

        Task<byte[]> GetLogoAsync(int tutorId);

    }
}
