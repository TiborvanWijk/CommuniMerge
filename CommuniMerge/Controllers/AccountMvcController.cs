using Microsoft.AspNetCore.Mvc;

namespace CommuniMerge.Controllers
{
    public class AccountMvcController : Controller
    {
        private readonly HttpClient client;

        public AccountMvcController(HttpClient httpClient)
        {
            this.client = httpClient;
            this.client.BaseAddress = new Uri("https://localhost:7129");
        }
        public IActionResult Login()
        {
            return View();
        }
        public async IActionResult Register()
        {
            var result = await 

            return View();
        }
    }
}
