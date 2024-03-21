using Microsoft.AspNetCore.Mvc;

namespace CommuniMerge.Controllers
{
    public class AuthorizeController : Controller
    {
        public AuthorizeController()
        {
            
        }
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Register()
        {
            return View();
        }
    }
}
