using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Company.Controllers
{
    public class LogistController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Logist")] // Защищаем метод авторизацией по роли
        public IActionResult Tickets()
        {
            return View();
        }

        [Authorize(Roles = "Logist")] // Защищаем метод авторизацией по роли
        public IActionResult Routes()
        {
            return View();
        }

        [Authorize(Roles = "Logist")] // Защищаем метод авторизацией по роли
        public IActionResult Flights()
        {
            return View();
        }

        [Authorize(Roles = "Logist")] // Защищаем метод авторизацией по роли
        public IActionResult Buses()
        {
            return View();
        }

        [Authorize(Roles = "Logist")] // Защищаем метод авторизацией по роли
        public IActionResult Drivers()
        {
            return View();
        }
    }
}
