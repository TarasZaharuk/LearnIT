using LearnIT.Application.DTOs;

namespace LearnIT.Application.Interfaces.Services
{
    public interface IConsultationsService
    {
        Task<int> AddAsync(AddConsultationModel consultation);

        Task AddAsync(List<AddConsultationModel> consultations);

        Task DeleteByIdAsync(int id);

        Task DeleteAllAsync();

        Task<List<ConsultationDTO>> GetAllAsync();

        Task<ConsultationDTO?> GetByIdAsync(int id);

        Task<List<ConsultationDTO>> GetByUserIdAsync(int userId);

        Task<List<ConsultationDTO>> GetByTutorIdAsync(int tutorId);

        Task<List<ConsultationDTO>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task UpdateAsync(UpdateConsultationModel consultation);
    }
}
