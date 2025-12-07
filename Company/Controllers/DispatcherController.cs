using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Company.Controllers
{
    public class DispatcherController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "dispatcher")] // Защищаем метод авторизацией по роли
        public IActionResult Sale()
        {
            return View();
        }

        [Authorize(Roles = "dispatcher")] // Защищаем метод авторизацией по роли
        public IActionResult ReturnTicket()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "dispatcher")] // Защищаем метод авторизацией по роли
        public IActionResult PassportDate()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "dispatcher")] // Защищаем метод авторизацией по роли
        public IActionResult Payment()
        {
            return View();
        }
    }
}
