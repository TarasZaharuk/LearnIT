using LearnIT.Application.Interfaces.Services.UsersEmailService;
using Microsoft.AspNetCore.Mvc;
using System.Configuration;

namespace LearnIT.WebUI.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class EmailController : Controller
    {
        private readonly IEmailConfirmationService _emailConfirmationService;
        private readonly string _homePageAddress;
        private const string HomePageAddressSection = "HomeFrontedPageAddress";

        public EmailController(IEmailConfirmationService emailConfirmationService, IConfiguration configuration)
        {
            _emailConfirmationService = emailConfirmationService;
            _homePageAddress = configuration[HomePageAddressSection] ?? throw new ConfigurationErrorsException($"'{HomePageAddressSection}' does not exist");
        }

        [HttpGet("/email/{token}")]
        public async Task<IActionResult> ConfirmEmail([FromRoute] string token)
        {
            string emailVerificationMessage = await _emailConfirmationService.ConfirmEmailAsync(token);
            //return standardized message instead of text
            return Ok(emailVerificationMessage);
        }
    }
}
