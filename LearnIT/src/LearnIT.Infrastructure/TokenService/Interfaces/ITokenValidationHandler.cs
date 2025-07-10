using LearnIT.Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LearnIT.Infrastructure.TokenService.Interfaces
{
    internal interface ITokenValidationHandler
    {
        TokenValidationResponse Handle(TokenValidationContext validationContext);
        void SetNext(ITokenValidationHandler handler);
    }
}
