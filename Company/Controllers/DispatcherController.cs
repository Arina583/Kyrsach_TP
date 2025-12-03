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
