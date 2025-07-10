using LearnIT.Application.Models;
using LearnIT.Infrastructure.TokenService.Interfaces;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;

namespace LearnIT.Infrastructure.TokenService.TokenValidationHandlers
{
    internal class TokenDecryptionHandler : TokenValidationHandler
    {
        private readonly TokenValidationParameters _validationParameters;
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public TokenDecryptionHandler(byte[] jwtKey)
        {
            _validationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false, // Ensure the token has not expired
                IssuerSigningKey = new SymmetricSecurityKey(jwtKey), // Key used to sign the token
                ValidateIssuerSigningKey = true
            };
            _tokenHandler = new JwtSecurityTokenHandler();
        }

        public override TokenValidationResponse Handle(TokenValidationContext context)
        {
            try
            {
                context.ClaimsPrincipal = _tokenHandler.ValidateToken(context.Token, _validationParameters, out var validatedToken);
                context.ValidatedToken = validatedToken;
                return base.Handle(context);
            }
            catch (SecurityTokenValidationException)
            {
                return new TokenValidationResponse(TokenValidationProblems.SecurityTokenInvalid);
            }
        }
    }
}
