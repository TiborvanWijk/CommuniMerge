using CommuniMerge.Library.Data.Dtos;
using CommuniMerge.Library.Enums;
using CommuniMerge.Library.Mappers;
using CommuniMerge.Library.Models;
using CommuniMerge.Library.ResultObjects;
using CommuniMerge.Library.Services;
using CommuniMerge.Library.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Communimerge.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IAccountService accountService;
        private readonly TokenService tokenService;

        public AccountController(IAccountService accountService, TokenService tokenService)
        {
            this.accountService = accountService;
            this.tokenService = tokenService;
        }


        [HttpPost("login")]
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

            var loginResult = await accountService.LoginAsync(loginModel);

            if (loginResult.Error == LoginError.InvalidCombination)
            {
                return BadRequest("Invalid combination");
            }
            var user = await accountService.GetUserByUsernameAsync(loginModel.Username);
            string bearerToken = tokenService.GenerateBearerToken(user.Id, user.UserName);

            var loginResponseDto = new LoginResponseDto() { Type = "Bearer", Token = bearerToken };

            return Ok(loginResponseDto);
        }

        [HttpPost("register")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> Register([FromBody] RegisterModel registerModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var registrationResult = await accountService.RegisterAsync(registerModel);

            if (registrationResult.Error != RegistrationError.None)
            {
                //TEMP NEEDS UPDATING
                return StatusCode(501, "THIS IS TEMPORARLY NOT IMPLEMENTED");
            }

            return Ok();
        }
    }
}
