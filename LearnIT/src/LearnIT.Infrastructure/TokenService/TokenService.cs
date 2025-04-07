using LearnIT.Application.DTOs;
using LearnIT.Application.Interfaces;
using LearnIT.Application.Models;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LearnIT.Infrastructure.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly byte[] _jwtKey;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly TokenValidationParameters _validationParameters;
        private readonly string _issuer;
        private readonly string _audience;

        public TokenService(IConfiguration configuration)
        {
            string jwtSecret = configuration["Jwt:Key"] ?? throw new ConfigurationErrorsException("'Jwt' section does not exist or 'Jwt:Key' value is null");
            _issuer = configuration["Jwt:Issuer"] ?? throw new ConfigurationErrorsException("'Jwt' section does not exist or 'Jwt:Issuer' value is null");
            _audience = configuration["Jwt:Audience"] ?? throw new ConfigurationErrorsException("'Jwt' section does not exist or 'Jwt:Audience' value is null");
            _jwtKey = Encoding.UTF8.GetBytes(jwtSecret);

            _tokenHandler = new JwtSecurityTokenHandler();//di
            _validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false, // Ensure the token has not expired
                IssuerSigningKey = new SymmetricSecurityKey(_jwtKey), // Key used to sign the token
                ValidateIssuerSigningKey = true
            };
        }

        public TokenValidationProblems TryValidateToken(string token, out int userId)
        {
            userId = -1;
            ClaimsPrincipal claimsPrincipal;
            SecurityToken validatedToken;
            try
            {
                claimsPrincipal = _tokenHandler.ValidateToken(token, _validationParameters, out validatedToken);
            }
            catch (SecurityTokenValidationException)
            {
                return TokenValidationProblems.SecurityTokenInvalid;
            }

            if (!TryGetUserIdFromClaims(claimsPrincipal, out userId))
                return TokenValidationProblems.SecurityTokenInvalid;

            if (validatedToken.ValidTo < DateTime.Now)
                return TokenValidationProblems.Expired;

            return TokenValidationProblems.None;
        }

        public string GenerateEmailConfirmationToken(string userEmail, string userId)
        {
            IEnumerable<Claim> emailConfirmationClaims = GetUserEmailConfirmationClaims(userEmail, userId);
            var tokenDescriptor = GetSecurityTokenDescriptor(emailConfirmationClaims, DateTime.UtcNow.AddDays(2));
            SecurityToken token = _tokenHandler.CreateToken(tokenDescriptor);
            return _tokenHandler.WriteToken(token);
        }

        public string GenerateAuthenticationToken(UserDTO user)
        {
            IEnumerable<Claim> userClaims = GetUserClaims(user);
            var tokenDescriptor = GetSecurityTokenDescriptor(userClaims, DateTime.UtcNow.AddMinutes(120));//
            SecurityToken token = _tokenHandler.CreateToken(tokenDescriptor);
            return _tokenHandler.WriteToken(token);
        }

        private SecurityTokenDescriptor GetSecurityTokenDescriptor(IEnumerable<Claim> claims, DateTime? expires)
        {
            return new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expires,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_jwtKey), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _issuer,
                Audience = _audience
            };
        }

        private IEnumerable<Claim> GetUserClaims(UserDTO user)
        {
            var claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim("TutorId", user.TutorId?.ToString() ?? string.Empty),
                new Claim(ClaimTypes.Role, user.TutorId.HasValue ? "Tutor" : "User")
            };

            return claims;
        }

        private IEnumerable<Claim> GetUserEmailConfirmationClaims(string email, string userIdentifier)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userIdentifier),
                new Claim(ClaimTypes.Email, email)
            };

            return claims;
        }

        private bool TryGetUserIdFromClaims(ClaimsPrincipal claims, out int userId)
        {
            string? userIdClaim = claims.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (int.TryParse(userIdClaim, out userId))
                return true;
            return false;
        }
    }
}
