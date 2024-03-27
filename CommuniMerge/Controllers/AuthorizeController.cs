using CommuniMerge.Library.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using CommuniMerge.Library.Data.Dtos;
using System.Net;
using Azure;
using CommuniMerge.Library.Mappers;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace CommuniMerge.Controllers
{
    public class AuthorizeController : Controller
    {
        private readonly HttpClient client;

        public AuthorizeController()
        {
            this.client = new HttpClient();
            this.client.BaseAddress = new Uri("https://localhost:7129");
        }
        public IActionResult Login()
        {
            return View(new LoginModel() { Username = "", Password = ""});
        }
        [HttpPost]
        public async Task<IActionResult> Login(LoginModel model)
        {

            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/login", jsonContent);

            if(response.StatusCode != HttpStatusCode.OK)
            {
                return View(model);
            }

            var resultContent = JsonConvert.DeserializeObject<LoginResponseDto>(await response.Content.ReadAsStringAsync());

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

        public async Task<IActionResult> Register()
        {
            return View(new RegisterModel() { Email = "", Password = "", Username = "" });
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel registerModel)
        {
            var registerJson = new StringContent(JsonConvert.SerializeObject(registerModel), Encoding.UTF8, "application/json"); 
            var result = await client.PostAsync("/register", registerJson);

            if(result.StatusCode != HttpStatusCode.OK)
            {
                return View(registerModel);
            }

            var loginModel = Map.ToLoginModelFromRegisterModel(registerModel);
            var loginJson = new StringContent(JsonConvert.SerializeObject(loginModel), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/login", loginJson);

            var resultContent = JsonConvert.DeserializeObject<LoginResponseDto>(await response.Content.ReadAsStringAsync());

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
    }
}
