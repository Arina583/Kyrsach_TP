using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Company.Models;
using System.Collections.Generic;
using System.Linq;

namespace TruckingCompany.Controllers
{
    public class FlightController : Controller
    {
        private readonly DbClassContext _context;

        public FlightController(DbClassContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Search(string from, string to)
        {
            TempData["From"] = from;
            TempData["To"] = to;

            // Получаем Id города отправления
            var departureStopId = _context.Stops.FirstOrDefault(s => s.nameStop == from)?.Id;
            // Получаем Id города прибытия
            var arrivalStopId = _context.Stops.FirstOrDefault(s => s.nameStop == to)?.Id;

            // Если хотя бы один из городов не найден, возвращаем пустой список рейсов
            if (departureStopId == null || arrivalStopId == null)
            {
                return View(new List<Flights>());
            }

            // Ищем рейсы, у которых маршрут начинается с города отправления и заканчивается городом прибытия
            var flights = _context.Flights
                .Include(f => f.Route)
                    .ThenInclude(r => r.cityDeparture) // Include города отправления
                .Include(f => f.Route)
                    .ThenInclude(r => r.cityArrival)   // Include города прибытия
                .Where(f => f.Route.DepartureStopsId == departureStopId && f.Route.ArrivalStopsId == arrivalStopId)
                .ToList();

            return View(flights);
        }

        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = _context.Flights
                .Include(f => f.Route)
                    .ThenInclude(r => r.cityDeparture)
                .Include(f => f.Route)
                    .ThenInclude(r => r.cityArrival)
                .FirstOrDefault(m => m.Id == id);

            if (flight == null)
            {
                return NotFound();
            }

            return View(flight);
        }

        public IActionResult BuyTicket(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = _context.Flights
                .Include(f => f.Route)
                    .ThenInclude(r => r.cityDeparture)
                .Include(f => f.Route)
                    .ThenInclude(r => r.cityArrival)
                .FirstOrDefault(m => m.Id == id);

            if (flight == null)
            {
                return NotFound();
            }

            var reservedSeats = _context.Tickets
                .Where(t => t.FlightId == id && t.status == "Забронирован")
                .Select(t => t.numberSeat)
                .ToList();

            ViewBag.ReservedSeats = reservedSeats;

            return View(flight);
        }

        [HttpPost]
        public async Task<IActionResult> ReserveSeat(int id, int seatNumber, string actionType, string email)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null || seatNumber <= 0 || seatNumber > 39) return NotFound();

            // Проверка, что место не занято
            if (await _context.Tickets.AnyAsync(t => t.FlightId == id && t.numberSeat == seatNumber && t.status != "available"))
            {
                return BadRequest("This seat is currently unavailable.");
            }

            if (actionType == "reserve")
            {
                // Получаем маршрут связанный с рейсом
                var route = await _context.Routes.FindAsync(flight.RouteId);
                if (route == null)
                {
                    return BadRequest("Route not found for this flight.");
                }

                // Предположим, что в маршруте есть поле DepartureStopId с Id начальной остановки
                var departureStopId = route.DepartureStopsId;

                var ticket = new Tickets
                {
                    FlightId = id,
                    numberSeat = seatNumber,
                    price = 215.0,   // Цена из рейса
                    status = "Забронирован",
                    email = email,
                    PassengerId = 1,        // Подставьте нужный PassengerId
                    StopId = departureStopId // Стартовая остановка из маршрута
                };

                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();

                // Задача на удаление брони через 30 минут
                Task.Delay(TimeSpan.FromMinutes(30)).ContinueWith(async _ =>
                {
                    var ticketToDelete = await _context.Tickets.FindAsync(ticket.id);
                    if (ticketToDelete != null && ticketToDelete.status == "Забронирован")
                    {
                        _context.Tickets.Remove(ticketToDelete);
                        await _context.SaveChangesAsync();
                    }
                });
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}
