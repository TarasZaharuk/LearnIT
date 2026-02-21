using LearnIT.Domain.Entities;

namespace LearnIT.Application.Interfaces.Repositories
{
    public interface IConsultationsRepository
    {
        Task<int> AddAsync(Consultation consultation);

        Task AddAsync(List<Consultation> consultations);

        Task DeleteAsync(int id);

        Task DeleteAllAsync();

        Task<List<Consultation>> GetAllAsync();

        Task<Consultation?> GetByIdAsync(int id);

        Task<List<Consultation>> GetByAttendeeIdAsync(int attendeeId);

        Task<List<Consultation>> GetByTutorIdAsync(int tutorId);

        Task<List<Consultation>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        Task UpdateAsync(Consultation updatedConsultation);
    }
}
