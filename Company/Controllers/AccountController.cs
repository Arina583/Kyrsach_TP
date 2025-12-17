using Company.Models;
using Company.Tests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TruckingCompany.Controllers
{
    public class AccountController : Controller
    {

        private readonly DbClassContext _context;

        public AccountController(DbClassContext context)
        {
            _context = context;
        }


        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string login, string password)
        {
            // 1. Проверка наличия пользователя в БД
            var user = _context.PersonalRoles.FirstOrDefault(u => u.login == login && u.password == password);

            if (user != null)
            {
                // 2. Создание claims для аутентификации
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.login),
                    new Claim(ClaimTypes.Role, user.role)
                };

                // 3. Создание identity
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 4. Аутентификация пользователя
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(identity));

                // 5. Редирект на нужную страницу в зависимости от роли
                if (user.role == "dispatcher")
                {
                    return RedirectToAction("DispatcherDashboard", "Account");
                }
                else if (user.role == "logist")
                {
                    return RedirectToAction("LogistPanel", "Account");
                }
                else
                {
                    return RedirectToAction("Index", "Home"); // Редирект на главную, если роль неизвестна
                }
            }

            // Если аутентификация не удалась
            ModelState.AddModelError("", "Неправильный логин или пароль."); // Сообщение об ошибке
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        [Authorize(Roles = "dispatcher")] // Защищаем метод авторизацией по роли
        public IActionResult DispatcherDashboard()
        {
            return View();
        }

        [Authorize(Roles = "logist")] // Защищаем метод авторизацией по роли
        public IActionResult LogistPanel()
        {
            return View();
        }
    }
}
