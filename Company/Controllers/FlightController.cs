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

            return View(flight);
        }

        [HttpPost]
        public IActionResult ReserveSeat([FromBody] ReserveRequest request)
        {
            // Проверить модель
            if (request == null || request.SeatNumber <= 0 || string.IsNullOrEmpty(request.Email))
                return Json(new { success = false, message = "Некорректные данные" });

            // TODO: проверить что место не занято
            bool isAvailable = CheckSeatAvailability(request.FlightId, request.SeatNumber);
            if (!isAvailable)
                return Json(new { success = false, message = "Место уже занято" });

            // TODO: сохранить бронирование (с пометкой, что оно в резерве)
            var reservation = SaveReservation(request.FlightId, request.SeatNumber, request.Email);

            // TODO: отправить email с реквизитами для оплаты на request.Email
            SendEmailPaymentDetails(request.Email, reservation);

            // URL страницы оплаты (можно параметр или хардкод)
            string paymentUrl = Url.Action("Payment", "Payment", new { reservationId = reservation.Id }, Request.Scheme);

            return Json(new { success = true, paymentUrl });
        }

        private void SendEmailPaymentDetails(string email, object reservation)
        {
            throw new NotImplementedException();
        }

        public class ReserveRequest
        {
            public int FlightId { get; set; }
            public int SeatNumber { get; set; }
            public string Email { get; set; }
        }
    }
}
