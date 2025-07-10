using LearnIT.Application.Models;

namespace LearnIT.Application.Interfaces.Services.UsersEmailService
{
    public interface IEmailSender
    {
        public Task<EmailSendResult> SendEmailWithHtmlBodyAsync(string toEmail, string subject, string htmlBody);
    }
}
