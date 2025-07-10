using LearnIT.Application.Models;
using LearnIT.Infrastructure.TokenService.Interfaces;
using System.Security.Claims;

namespace LearnIT.Infrastructure.TokenService.TokenValidationHandlers
{
    internal class UserIdExtractionHandler : TokenValidationHandler
    {
        public override TokenValidationResponse Handle(TokenValidationContext context)
        {
            if (!TryGetUserIdFromClaims(context.ClaimsPrincipal, out int userId))
            {
                return new TokenValidationResponse(TokenValidationProblems.SecurityTokenInvalid);
            }

            context.UserId = userId;
            return base.Handle(context);
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
