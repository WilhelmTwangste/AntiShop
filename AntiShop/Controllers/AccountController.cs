using AntiShop.Models.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Collections.Generic;
using ToDoList.Domain;
using ToDoList.Domain.Entites;
using Microsoft.AspNetCore.Authentication;
using AntiShop.Models;
using Microsoft.AspNetCore.Identity;

namespace AntiShop.Controllers
{
    public class AccountController : TodoBaseController
    {
        private readonly ToDoListContext _context;
        public AccountController(ToDoListContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }


        [HttpGet]
        public IActionResult Register()
        {
            return View(new RegisterViewModel());
        }


        [HttpPost]
        public async Task<IActionResult> LoginAsync([Bind(Prefix = "l")] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", new AccountViewModel
                {
                    LoginViewModel = model
                });
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Login == model.Login && u.Password == model.Password);
            if (user is null)
            {
                ViewBag.Error = "Некорректные логин и(или) пароль";
                return View("Index", new AccountViewModel
                {
                    LoginViewModel = model
                });
            }

            await AuthenticateAsync(user);

            // Перенаправление в зависимости от роли
            if (user.Role == "Admin")
            {
                return RedirectToAction("Index", "Admin"); // Страница администратора
            }
            else if (user.Role == "Accountant")
            {
                return RedirectToAction("Index", "Accountant"); // Страница бухгалтера
            }
            else if (user.Role == "WarehouseKeeper")
            {
                return RedirectToAction("Index", "WarehouseKeeper"); // Страница товароведа
            }

            return RedirectToAction("Index", "Home"); // По умолчанию
        }

        private async Task AuthenticateAsync(User user)
        {
            var claims = new List<Claim>
            {
                new Claim (ClaimTypes.NameIdentifier, user. Id.ToString()),
                new Claim (ClaimsIdentity.DefaultNameClaimType, user.Login),
                new Claim(ClaimTypes.Role, user.Role)
            };
            var id = new ClaimsIdentity(claims, "ApplicationCookie", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }

        private async Task<IActionResult> RegisterAsync([Bind(Prefix = "r")] RegisterViewModel model)
        {
            return RedirectToAction("Index", "Home");
        }
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction("Index", "Account"); // Перенаправление на страницу входа
            }
            catch (Exception ex)
            {
                return RedirectToAction("Index", "Home"); // Перенаправление на главную страницу в случае ошибки
            }
        }
    }
    
}
 