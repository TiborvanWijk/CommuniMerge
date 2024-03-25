using CommuniMerge.Library.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Communimerge.Api.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class AccountController : Controller
    {
        private readonly IAccountService userService;

        public AccountController(IAccountService accountService)
        {
            this.userService = accountService;
        }


        //[HttpPost("/login")]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(401)]
        //[ProducesResponseType(500)]
        //public async Task<IActionResult> Login([FromQuery] string username, [FromQuery] string password)
        //{
        //    if(!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

            

        //    return Ok();
        //}

        //[HttpPost("/register")]
        //[ProducesResponseType(200)]
        //[ProducesResponseType(400)]
        //[ProducesResponseType(500)]
        //public async Task<IActionResult> Register([FromQuery] string username, [FromQuery] string email, [FromQuery] string password)
        //{
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest(ModelState);
        //    }

            

        //    return Ok();
        //}
    }
}
