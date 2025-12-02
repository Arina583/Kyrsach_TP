using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TruckingCompany.Controllers
{
    public class AccountController : Controller
    {

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            // Проверка учетных данных
            if (username == "dispatcher" && password == "1234")
            {
                // Создание списка утверждений (Claims) для пользователя
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Dispatcher") // Добавление роли
                };

                // Создание удостоверения пользователя на основе утверждений
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Настройка параметров аутентификации
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // Запомнить пользователя между сессиями
                };

                // Выполнение аутентификации пользователя
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Перенаправление на страницу диспетчера
                return RedirectToAction("DispatcherDashboard", "Account");
            }

            if (username == "logist" && password == "12345")
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, "Logist") // Добавление роли
                };

                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("LogistPanel", "Account");
            }

            ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
            return View("Index");
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account"); // Перенаправляем на главную страницу после выхода
        }

        [Authorize(Roles = "Dispatcher")] // Защищаем метод авторизацией по роли
        public IActionResult DispatcherDashboard()
        {
            return View(); // Возвращаем представление с панелью диспетчера
        }

        [Authorize(Roles = "Dispatcher")] // Защищаем метод авторизацией по роли
        public IActionResult Sale()
        {
            return View();
        }

        [Authorize(Roles = "Dispatcher")] // Защищаем метод авторизацией по роли
        public IActionResult ReturnTicket()
        {
            return View();
        }
    }
}
