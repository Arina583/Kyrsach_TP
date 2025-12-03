using Microsoft.AspNetCore.Mvc;

namespace Company.Controllers
{
    public class LogistController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Tickets()
        {
            return View();
        }

        public IActionResult Routes()
        {
            return View();
        }

        public IActionResult Flights()
        {
            return View();
        }
        public IActionResult Buses()
        {
            return View();
        }

        public IActionResult Drivers()
        {
            return View();
        }
    }
}
