using LearnIT.Application.Models;

namespace LearnIT.Infrastructure.TokenService.Interfaces
{
    internal abstract class TokenValidationHandler : ITokenValidationHandler
    {
        private ITokenValidationHandler? _next;

        public void SetNext(ITokenValidationHandler handler)
        {
            _next = handler;
        }

        public virtual TokenValidationResponse Handle(TokenValidationContext context)
        {
            return _next?.Handle(context) ?? new TokenValidationResponse(context.UserId, TokenValidationProblems.None);
        }
    }
}
