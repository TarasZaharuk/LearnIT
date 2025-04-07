using System.Net.Mail;
using LearnIT.Application.Interfaces.Services.UsersEmailService;

namespace LearnIT.Infrastructure.EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpClient _smtpClient;

        public EmailSender(SmtpClient smtpClient)
        {
            _smtpClient = smtpClient;
        }

        public async Task SendEmailWithHtmlBodyAsync(string toEmail, string subject, string htmlBody)
        {
            MailMessage mailMessage = new MailMessage
            {
                //magic string
                From = new MailAddress("taraszaharuk410@gmail.com"),
                Subject = subject,
                IsBodyHtml = true,
                Body = htmlBody,
            };
            mailMessage.To.Add(new MailAddress(toEmail));
            await _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
