namespace LearnIT.Application.Interfaces.Services.UsersEmailService
{
    public interface IEmailSender
    {
        public Task SendEmailWithHtmlBodyAsync(string toEmail, string subject, string htmlBody);
    }
}
