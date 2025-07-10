using LearnIT.Application.Models;
using LearnIT.Domain.Entities;

namespace LearnIT.Application.Interfaces.Services.UsersEmailService
{
    public interface IEmailConfirmationService
    {
        public Task<EmailSendingIssues> SendEmailConfirmationToAsync(int userId);

        public Task<string> ConfirmEmailAsync(string token);
    }
}
