using LearnIT.Application.Interfaces.Repositories;
using LearnIT.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LearnIT.Infrastructure.Persistence.Repositories;

public class ConsultationsRepository : IConsultationsRepository
{
    private readonly LearnITDBContext _learnITDBContext;

    public ConsultationsRepository(LearnITDBContext learnITDBContext)
    {
        _learnITDBContext = learnITDBContext;
    }

    public async Task<int> AddAsync(Consultation consultation)
    {
        await _learnITDBContext.Consultations.AddAsync(consultation);
        await _learnITDBContext.SaveChangesAsync();
        return consultation.Id;
    }

    public async Task AddAsync(List<Consultation> consultations)
    {
        await _learnITDBContext.Consultations.AddRangeAsync(consultations);
        await _learnITDBContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        Consultation? consultation = await _learnITDBContext.Consultations.FindAsync(id);
        if (consultation == null)
            return;

        _learnITDBContext.Consultations.Remove(consultation);
        await _learnITDBContext.SaveChangesAsync();
    }

    public async Task DeleteAllAsync()
    {
        await _learnITDBContext.Consultations.ExecuteDeleteAsync();
        await _learnITDBContext.SaveChangesAsync();
    }

    public async Task<List<Consultation>> GetAllAsync()
    {
        return await _learnITDBContext.Consultations
            .Include(c => c.Attendee)
            .Include(c => c.Tutor)
            .ToListAsync();
    }

    public async Task<Consultation?> GetByIdAsync(int id)
    {
        return await _learnITDBContext.Consultations
            .Include(c => c.Attendee)
            .Include(c => c.Tutor)
            .SingleOrDefaultAsync(c => c.Id == id);
    }

    public async Task<List<Consultation>> GetByAttendeeIdAsync(int attendeeId)
    {
        return await _learnITDBContext.Consultations
            .Include(c => c.Attendee)
            .Include(c => c.Tutor)
            .Where(c => c.AttendeeId == attendeeId)
            .ToListAsync();
    }

    public async Task<List<Consultation>> GetByTutorIdAsync(int tutorId)
    {
        return await _learnITDBContext.Consultations
            .Include(c => c.Attendee)
            .Include(c => c.Tutor)
            .Where(c => c.TutorId == tutorId)
            .ToListAsync();
    }

    public async Task<List<Consultation>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _learnITDBContext.Consultations
            .Include(c => c.Attendee)
            .Include(c => c.Tutor)
            .Where(c => c.Start >= startDate && c.Start <= endDate)
            .ToListAsync();
    }

    public async Task UpdateAsync(Consultation updatedConsultation)
    {
        _learnITDBContext.Consultations.Update(updatedConsultation);
        await _learnITDBContext.SaveChangesAsync();
    }
}

