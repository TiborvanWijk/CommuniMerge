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

            switch (loginResult.Error)
            {
                case LoginError.None:
                    break;
                case LoginError.InvalidCombination:
                    return BadRequest("Invalid combination");
                case LoginError.UnExpected:
                    return StatusCode(500, "Unexpected server error.");
                default:
                    return StatusCode(500, "Unknown server error.");
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


            switch (registrationResult.Error)
            {
                case RegistrationError.None:
                    break;
                case RegistrationError.EmailExists:
                    return BadRequest("Email alreay in use.");
                case RegistrationError.InvalidEmailFormat:
                    return BadRequest("Invalid email format.");
                case RegistrationError.WeakPassword:
                    return BadRequest("Password must be at least 8 characters long and contain at least one special character");
                case RegistrationError.CreateUserFailed:
                    return StatusCode(500, "Something went wrong while registering");
                case RegistrationError.UnknownError:
                    return StatusCode(500, "Unexpected server error.");
                default:
                    return StatusCode(500, "Unknown server error.");
            }

            return Created();
        }
    }
}
