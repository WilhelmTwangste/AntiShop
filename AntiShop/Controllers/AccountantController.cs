using Microsoft.AspNetCore.Mvc;

namespace AntiShop.Controllers
{
    public class AccountantController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
