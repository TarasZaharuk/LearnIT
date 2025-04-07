using LearnIT.Application.DTOs;
using LearnIT.Application.Models;

namespace LearnIT.Application.Interfaces
{
    public interface ITokenService
    {
        TokenValidationProblems TryValidateToken(string token, out int userId);
        string GenerateEmailConfirmationToken(string userEmail, string userId);

        string GenerateAuthenticationToken(UserDTO user);
    }
}
