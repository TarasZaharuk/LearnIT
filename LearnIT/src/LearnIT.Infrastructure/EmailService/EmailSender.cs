using System.Net.Mail;
using LearnIT.Application.Interfaces.Services.UsersEmailService;
using LearnIT.Application.Models;
using EmailValidation;
using Microsoft.Extensions.Configuration;
using System.Configuration;

namespace LearnIT.Infrastructure.EmailService
{
    public class EmailSender : IEmailSender
    {
        private readonly SmtpClient _smtpClient;
        private readonly EmailValidator _emailValidator;

        private readonly string _fromMail;
        public EmailSender(SmtpClient smtpClient, IConfiguration configuration)
        {
            _smtpClient = smtpClient;
            _emailValidator = new EmailValidator();
            _fromMail = configuration["SenderEmail"] ?? throw new ConfigurationErrorsException("SenderEmail is not found");
        }

        public async Task<EmailSendResult> SendEmailWithHtmlBodyAsync(string toEmail, string subject, string htmlBody)
        {
            bool isToEmailValid = IsEmailValid(toEmail);
            if (!isToEmailValid)
            {
                return new EmailSendResult(EmailSendingIssues.InvalidRecipient,
                       $"Recipient does not exist: {toEmail}");
            }
                
            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress(_fromMail),
                Subject = subject,
                IsBodyHtml = true,
                Body = htmlBody,
                DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure
            };
            mailMessage.To.Add(new MailAddress(toEmail));

            try
            {
                await _smtpClient.SendMailAsync(mailMessage);
                return new EmailSendResult(EmailSendingIssues.None);
            }
            catch (SmtpException)
            {
                return new EmailSendResult(EmailSendingIssues.SmtpProtocolError);
            }
        }

        private bool IsEmailValid(string email)
        {
            EmailValidationResult validationResult;

            _emailValidator.Validate(email, out validationResult);
            return validationResult == EmailValidationResult.OK;
        }
    }
}
