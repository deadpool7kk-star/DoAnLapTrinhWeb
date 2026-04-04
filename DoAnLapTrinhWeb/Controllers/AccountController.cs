using Microsoft.AspNetCore.Mvc;

namespace DoAnLapTrinhWeb.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
