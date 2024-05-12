using CommuniMerge.Library.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using Azure;
using CommuniMerge.Library.Mappers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using CommuniMerge.ViewModels;
using CommuniMerge.ApiServices.Interfaces;
using CommuniMerge.Library.ResultObjects;

namespace CommuniMerge.Controllers
{
    public class AuthorizeController : Controller
    {
        private readonly IApiService apiService;

        public AuthorizeController(IApiService apiService)
        {
            this.apiService = apiService;
        }
        public IActionResult Login()
        {
            var loginModel = new LoginModel() { Username = "", Password = "" };
            return View(new LoginView { LoginModel = loginModel, FeedbackMessage = null });
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {

            var response = await apiService.SendHttpRequest<LoginModel>(HttpContext, "/api/Account/login", HttpMethod.Post, loginModel);
            if (response.StatusCode != HttpStatusCode.OK)
            {
                var FeedbackMessage = await response.Content.ReadAsStringAsync();
                return View(new LoginView { LoginModel = loginModel, FeedbackMessage = FeedbackMessage });
            }

            var resultContent = JsonConvert.DeserializeObject<LoginResponseDto>(await response.Content.ReadAsStringAsync());

            var cookieOptions = new CookieOptions()
            {
                Path = "/",
                Expires = null,
                HttpOnly = true,
                Secure = true,
            };
            Response.Cookies.Append("BearerToken", resultContent.Token, cookieOptions);

            return Redirect("/");
        }

        public async Task<IActionResult> Register()
        {
            var registerModel = new RegisterModel() { Email = "", Password = "", Username = "" };
            return View(new RegisterView { RegisterModel = registerModel, FeedbackMessage = null});
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {

            var registerResult = await apiService.SendHttpRequest<RegisterModel>(HttpContext, "/api/Account/register", HttpMethod.Post, registerModel);

            if (registerResult.StatusCode != HttpStatusCode.NoContent)
            {
                var feedbackMessage = await registerResult.Content.ReadAsStringAsync();
                return View(new RegisterView { RegisterModel = registerModel, FeedbackMessage = feedbackMessage});
            }

            var loginModel = Map.ToLoginModelFromRegisterModel(registerModel);
            var loginResult = await apiService.SendHttpRequest<LoginModel>(HttpContext, "/api/Account/login", HttpMethod.Post, loginModel);

            var resultContent = JsonConvert.DeserializeObject<LoginResponseDto>(await loginResult.Content.ReadAsStringAsync());

            if (resultContent.Type.Equals("Bearer"))
            {
                var cookieOptions = new CookieOptions()
                {
                    Path = "/",
                    Expires = DateTime.Now.AddHours(1),
                    HttpOnly = true,
                    Secure = true,
                };
                Response.Cookies.Append("BearerToken", resultContent.Token, cookieOptions);
            }

            return Redirect("/");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            Response.Cookies.Delete("BearerToken");

            return Redirect("/Authorize/login");
        }
    }
}
