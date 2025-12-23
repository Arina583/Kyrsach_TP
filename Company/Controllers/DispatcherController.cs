using Company.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;

namespace Company.Controllers
{
    public class DispatcherController : Controller
    {
        private readonly DbClassContext _context;

        public DispatcherController(DbClassContext context)
        {
            _context = context;
        }

        // GET: /Dispatcher/Sale
        // Показывает список доступных маршрутов
        public async Task<IActionResult> Sale()
        {
            // Загрузка маршрутов
            var routes = await _context.Routes
                .Include(r => r.cityDeparture)
                .Include(r => r.cityArrival)
                .ToListAsync();

            // Загрузка рейсов
            var flights = await _context.Flights.ToListAsync();

            // Создание единого списка объектов для представления
            var combinedData = new List<(string RouteInfo, string FlightInfo, TimeOnly DepartureTime)>();

            foreach (var route in routes)
            {
                foreach (var flight in flights.Where(f => f.RouteId == route.id))
                {
                    combinedData.Add((
                        $"{route.cityDeparture.nameStop} → {route.cityArrival.nameStop}",
                        flight.Id.ToString(),
                        flight.timeDeparture
                    ));
                }
            }

            return View(combinedData);
        }

        public async Task<IActionResult> Select(int id)
        {
            // Находим рейс по идентификатору
            var flight = await _context.Flights.FindAsync(id);

            if (flight == null)
                return Content("Рейс не найден.");

            // Находим автобус, связанный с рейсом
            var bus = await _context.Buses.FindAsync(flight.BusId);

            if (bus == null || bus.numberSeat == 0)
                return Content("Автобус не найден или вместимость неизвестна.");

            // Количество мест в автобусе
            var totalSeats = bus.numberSeat;

            // Занятые места
            var occupiedSeats = await _context.Tickets
                .Where(t => t.FlightId == flight.Id)
                .Select(t => t.numberSeat)
                .ToListAsync();

            // Свободные места
            var freeSeats = Enumerable.Range(1, totalSeats).Except(occupiedSeats).ToList();

            TempData["FlightId"] = flight.Id;

            return View(freeSeats);
        }


        // POST: покупка нового билета
        [HttpPost]
        public async Task<IActionResult> Buy(int seat)
        {
            var flightId = Convert.ToInt32(TempData["FlightId"]);

            var flight = await _context.Flights
                .Include(f => f.Route)
                    .ThenInclude(r => r.cityDeparture)
                .FirstOrDefaultAsync(f => f.Id == flightId);

            if (flight == null)
            {
                return Content("Рейс не найден.");
            }

            var ticket = new Tickets
            {
                numberSeat = seat,
                FlightId = flight.Id,
                status = "Куплен",
                PassengerId = 1,
                StopId = flight.Route.cityDeparture.Id
            };

            _context.Add(ticket);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(PrintTicket), new { ticketId = ticket.id });
        }


        // GET: /Dispatcher/PrintTicket/{ticketId}
        // Показывает билет для печати
        public async Task<IActionResult> PrintTicket(int ticketId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Flight)
                .ThenInclude(f => f.Route)
                .FirstOrDefaultAsync(t => t.id == ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }


        // Заявка на возврат билета
        public IActionResult RequestRefund(int ticketId)
        {
            var ticket = _repository.GetTicketById(ticketId);
            return View(ticket);
        }

        // Обработка заявки на возврат
        [HttpPost]
        public IActionResult ProcessRefund(RequestRefundModel model)
        {
            _repository.RequestRefund(model.TicketId, model.Reason);
            return RedirectToAction("Index", "Home");
        }
    }
}
