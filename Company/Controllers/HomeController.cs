using Company.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Company.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [Authorize(Roles = "Dispatcher")] // Защищаем метод авторизацией по роли
        public IActionResult DispatcherDashboard()
        {
            return View(); // Возвращаем представление с панелью диспетчера
        }

        [Authorize(Roles = "Logist")] // Защищаем метод авторизацией по роли
        public IActionResult LogistPanel()
        {
            return View(); // Возвращаем представление с панелью диспетчера
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
