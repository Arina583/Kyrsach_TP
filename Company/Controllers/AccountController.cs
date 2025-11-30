using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TruckingCompany.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        public async Task<IActionResult> Login(string username, string password) // Получаем логин/пароль из формы
        {
            // TODO: Реальная проверка логина и пароля (обычно из базы данных)
            if (username == "admin" && password == "12345")
            {
                // Создаем Claims (информация о пользователе)
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, username), // Имя пользователя
                    //new Claim(ClaimTypes.Role, "Administrator")
                };

                // Создаем Identity (представление пользователя для аутентификации)
                var claimsIdentity = new ClaimsIdentity(
                    claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Аутентифицируем пользователя с использованием CookieAuthentication
                var authProperties = new AuthenticationProperties
                {
                    //AllowRefresh = <bool>,
                    //ExpiresUtc = DateTimeOffset.UtcNow.AddMinutes(10), // Время жизни куки
                    //IsPersistent = true, // Запомнить пользователя между сессиями
                    //IssuedUtc = <DateTimeOffset>,
                    //RedirectUri = <string>
                };
                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                // Перенаправляем на главную страницу после успешной авторизации
                return RedirectToAction("Index", "Account"); // "Index" - имя метода, "Account" - имя контроллера
            }

            // Если авторизация не удалась, возвращаем на страницу логина с сообщением об ошибке
            ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
            return View(); // Возвращаем представление Login.cshtml с ошибками
        }

        // GET: /Account/Logout
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Account"); // Перенаправляем на главную страницу после выхода
        }
    }
}
