using LearnIT.Application.DTOs;
using LearnIT.Application.Models;
using Shared;

namespace LearnIT.Application.Interfaces.Services
{
    public interface ITutorsService
    {
        Task<List<TutorDTO>> GetAsync(TutorsFilterModel tutorsFilter);

        Task<TutorDTO?> GetByIdAsync(int id);

        Task<int> AddAsync(AddTutorModel tutor);

        Task UpdateGeneralInfoAsync(UpdateTutorGeneralInfoModel tutor);

        Task DeleteByIdAsync(int id);

        Task DeleteAllAsync();

        Task UpdateSkillsAsync(AddTutorSkillsModel addTutorSkills);

        Task SetLogoAsync(int tutorId , byte[] logo);

        Task<byte[]> GetLogoAsync(int tutorId);
    }
}
