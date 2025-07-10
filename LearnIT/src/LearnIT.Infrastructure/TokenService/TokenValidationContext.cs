using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;

namespace LearnIT.Infrastructure.TokenService
{
    internal class TokenValidationContext
    {
        public string Token { get; set; } = null!;
        public ClaimsPrincipal ClaimsPrincipal { get; set; } = null!;
        public SecurityToken ValidatedToken { get; set; } = null!;
        public int UserId { get; set; }
    }
}
