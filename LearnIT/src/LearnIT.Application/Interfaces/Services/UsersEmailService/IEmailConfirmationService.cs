using LearnIT.Domain.Entities;

namespace LearnIT.Application.Interfaces.Services.UsersEmailService
{
    public interface IEmailConfirmationService
    {
        public Task SendEmailConfirmationToAsync(int userId);

        public Task<string> ConfirmEmailAsync(string token);
    }
}
