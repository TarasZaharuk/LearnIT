using LearnIT.Application.Interfaces.Repositories;
using LearnIT.Application.Interfaces;
using LearnIT.Application.Models;
using LearnIT.Domain.Entities;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using LearnIT.Application.Interfaces.Services.UsersEmailService;
using System.Reflection;

namespace LearnIT.Infrastructure.EmailService
{
    public class EmailConfirmationService : IEmailConfirmationService
    {
        private readonly ITokenService _tokenService;
        private readonly IEmailSender _emailSender;
        private readonly IUsersRepository _usersRepository;
        private readonly string _emailConfirmationHtmlFilePath;
        private readonly string _emailConfirmationControllerAddress;


        public EmailConfirmationService(ITokenService tokenService, IUsersRepository usersRepository, IEmailSender emailSender, IConfiguration configuration)
        {
            string basePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            string emailConfirmationHtmlFileName = configuration["EmailConfirmationHtmlBodyPath"] ?? throw new ConfigurationErrorsException("'EmailConfirmationHtmlTemplatePath' does not exist");
            _emailConfirmationHtmlFilePath = Path.Combine(basePath, emailConfirmationHtmlFileName);
            _tokenService = tokenService;
            _usersRepository = usersRepository;
            _emailSender = emailSender;
            _emailConfirmationControllerAddress = configuration["EmailConfirmationControllerAddress"] ?? throw new ConfigurationErrorsException("'EmailConfirmationControllerAddress' does not exist");
        }

        public async Task<string> ConfirmEmailAsync(string token)
        {
            TokenValidationResponse tokenValidationResponse = _tokenService.TryValidateToken(token);
            if (tokenValidationResponse.TokenValidationProblem is TokenValidationProblems.SecurityTokenInvalid)
                return "Your account has not been verified. Please try signing up again.";
            if (tokenValidationResponse.TokenValidationProblem is TokenValidationProblems.Expired)
            {
                await SendEmailConfirmationToAsync(tokenValidationResponse.UserId);
                return "Your confirmation email has expired. We have sent you a new confirmation email.";
            }

            var user = await _usersRepository.GetByIdAsync(tokenValidationResponse.UserId);
            if (user is null)
                return "No user with this email address was found. Please try signing up again.";

            user.EmailConfirmed = true;
            await _usersRepository.UpdateUserAsync(user);

            return "Your email address has been confirmed.";
        }

        public async Task<EmailSendingIssues> SendEmailConfirmationToAsync(int userId)
        {
            User user = await _usersRepository.GetByIdAsync(userId) ?? throw new Exception($"User with id:{userId} does not exist");

            string emailConfirmationToken = _tokenService.GenerateEmailConfirmationToken(user.Email, user.Id.ToString());
            string confirmationLink = $"{_emailConfirmationControllerAddress}/{emailConfirmationToken}";
            string htmlBody = GetEmailConfirmationBody(user.FirstName, confirmationLink);

            EmailSendResult sendResult = await _emailSender.SendEmailWithHtmlBodyAsync(user.Email, "Verify your email", htmlBody);
            return sendResult.SendingIssue;
        }

        private string GetEmailConfirmationBody(string userName, string confirmationLink)
        {

            string htmlBody = File.ReadAllText(_emailConfirmationHtmlFilePath)
                                .Replace("${first_name}", userName)
                                .Replace("${confirmation_link}", confirmationLink);
            return htmlBody;
        }
    }
}
