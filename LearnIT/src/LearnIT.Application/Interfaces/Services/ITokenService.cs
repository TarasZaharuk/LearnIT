using LearnIT.Application.DTOs;
using LearnIT.Application.Models;

namespace LearnIT.Application.Interfaces
{
    public interface ITokenService
    {
        TokenValidationResponse TryValidateToken(string token);
        string GenerateEmailConfirmationToken(string userEmail, string userId);

        string GenerateAuthenticationToken(UserDTO user);
    }
}
