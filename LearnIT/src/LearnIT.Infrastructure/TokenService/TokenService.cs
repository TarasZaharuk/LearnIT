using LearnIT.Application.DTOs;
using LearnIT.Application.Interfaces;
using LearnIT.Application.Models;
using Microsoft.IdentityModel.Tokens;
using System.Configuration;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LearnIT.Infrastructure.TokenService.Interfaces;
using LearnIT.Infrastructure.TokenService.TokenValidationHandlers;

namespace LearnIT.Infrastructure.TokenService
{
    public class TokenService : ITokenService
    {
        private readonly ITokenValidationHandler _handlerChain;
        private readonly byte[] _jwtKey;
        private readonly JwtSecurityTokenHandler _tokenHandler;
        private readonly string _issuer;
        private readonly string _audience;

        private readonly DateTime _emailConfirmationTokenExpires = DateTime.UtcNow.AddDays(2);
        private readonly DateTime _authenticationTokenExpires = DateTime.UtcNow.AddMinutes(120);

        public TokenService(IConfiguration configuration)
        {
            string jwtSecret = configuration["Jwt:Key"] ?? throw new ConfigurationErrorsException("'Jwt' section does not exist or 'Jwt:Key' value is null");
            _issuer = configuration["Jwt:Issuer"] ?? throw new ConfigurationErrorsException("'Jwt' section does not exist or 'Jwt:Issuer' value is null");
            _audience = configuration["Jwt:Audience"] ?? throw new ConfigurationErrorsException("'Jwt' section does not exist or 'Jwt:Audience' value is null");
            _jwtKey = Encoding.UTF8.GetBytes(jwtSecret);
            _tokenHandler = new JwtSecurityTokenHandler();

            var decryptionHandler = new TokenDecryptionHandler(_jwtKey);
            var userIdExtractionHandler = new UserIdExtractionHandler();
            var expirationHandler = new TokenExpirationHandler();

            decryptionHandler.SetNext(userIdExtractionHandler);
            userIdExtractionHandler.SetNext(expirationHandler);

            _handlerChain = decryptionHandler;
        }

        public TokenValidationResponse TryValidateToken(string token)
        {
            var context = new TokenValidationContext { Token = token };
            var response = _handlerChain.Handle(context);

            return response;
        }

        public string GenerateEmailConfirmationToken(string userEmail, string userId)
        {
            IEnumerable<Claim> emailConfirmationClaims = GetUserEmailConfirmationClaims(userEmail, userId);
            var tokenDescriptor = GetSecurityTokenDescriptor(emailConfirmationClaims, _emailConfirmationTokenExpires);
            SecurityToken token = _tokenHandler.CreateToken(tokenDescriptor);
            return _tokenHandler.WriteToken(token);
        }

        public string GenerateAuthenticationToken(UserDTO user)
        {
            IEnumerable<Claim> userClaims = GetUserClaims(user);
            var tokenDescriptor = GetSecurityTokenDescriptor(userClaims, _authenticationTokenExpires);
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
    }
}
