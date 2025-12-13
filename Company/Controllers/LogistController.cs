using Company.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Company.Controllers
{
    public class LogistController : Controller
    {
        private readonly DbClassContext _context;

        public LogistController(DbClassContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "logist")] // Защищаем метод авторизацией по роли
        public IActionResult Report()
        {
            return View();
        }

        /*[Authorize(Roles = "logist")] // Защищаем метод авторизацией по роли
        public IActionResult Routes()
        {
            return View();
        }*/

        [Authorize(Roles = "logist")]
        public IActionResult Schedule(string departureCity, string arrivalCity)
        {
            IQueryable<Flights> flights = _context.Flights
                           .Include(f => f.Route)
                               .ThenInclude(r => r.cityDeparture)
                           .Include(f => f.Route)
                               .ThenInclude(r => r.cityArrival);

            if (!string.IsNullOrEmpty(departureCity))
            {
                flights = flights.Where(f => f.Route.cityDeparture.nameStop.Contains(departureCity));
            }

            if (!string.IsNullOrEmpty(arrivalCity))
            {
                flights = flights.Where(f => f.Route.cityArrival.nameStop.Contains(arrivalCity));
            }

            ViewBag.DepartureCity = departureCity;
            ViewBag.ArrivalCity = arrivalCity;

            return View(flights.ToList());
        }

        [Authorize(Roles = "logist")] // Защищаем метод авторизацией по роли
        public IActionResult Buses()
        {
            return View(_context.Buses.ToList());
        }

        [Authorize(Roles = "logist")]
        public IActionResult Drivers()
        {
            // Получаем список водителей из базы данных
            return View(_context.Drivers.ToList());
        }
    }
}
