using CommuniMerge.Library.Enums;
using CommuniMerge.Library.Models;
using CommuniMerge.Library.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Communimerge.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IAccountService accountService;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }


        [HttpPost("/login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Login([FromBody] LoginModel loginModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }



            return Ok();
        }

        [HttpPost("/register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var registrationResult = await accountService.Register(registerModel);

            if (registrationResult.Error != RegistrationError.None)
            {
                //TEMP NEEDS UPDATING
                return BadRequest();
            }

            return Ok();
        }
    }
}
