using CommuniMerge.Library.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;

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
            return View(model);
        }

        public async Task<IActionResult> Register()
        {
            return View(new RegisterModel() { Email = "", Password = "", Username = "" });
        }
        [HttpPost]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var jsonContent = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json"); 
            var result = await client.PostAsync("/register", jsonContent);
            var code = result.StatusCode;

            return View(model);
        }
    }
}
