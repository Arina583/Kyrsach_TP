using Company.Models;
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
            // 1. Сначала проверяем валидность входных данных
            if (string.IsNullOrEmpty(login))
            {
                ModelState.AddModelError("login", "Логин не может быть пустым.");
                return View();
            }

            if (string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("password", "Пароль не может быть пустым.");
                return View();
            }

            // 2. Проверка наличия пользователя в БД
            var user = _context.PersonalRoles
                .FirstOrDefault(u => u.login == login && u.password == password);

            if (user != null)
            {
                // 3. Создание claims для аутентификации
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.login),
            new Claim(ClaimTypes.Role, user.role)
        };

                // 4. Создание identity
                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // 5. Аутентификация пользователя
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(identity));

                // 6. Редирект на нужную страницу в зависимости от роли
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
                    return RedirectToAction("Index", "Home");
                }
            }

            // Если аутентификация не удалась
            ModelState.AddModelError("", "Неправильный логин или пароль.");
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
