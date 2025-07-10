using LearnIT.Application.Models;
using LearnIT.Infrastructure.TokenService.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnIT.Infrastructure.TokenService.TokenValidationHandlers
{
    internal class TokenExpirationHandler : TokenValidationHandler
    {
        public override TokenValidationResponse Handle(TokenValidationContext context)
        {
            if (context.ValidatedToken.ValidTo < DateTime.Now)
            {
                return new TokenValidationResponse(context.UserId, TokenValidationProblems.Expired);
            }

            return base.Handle(context);
        }
    }
}
