using Company.Models;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "dispatcher")]
        public async Task<IActionResult> Sale()
        {
            // Загружаем маршруты
            var routes = await _context.Routes
                .Include(r => r.cityDeparture)
                .Include(r => r.cityArrival)
                .ToListAsync();

            // Загружаем активные рейсы (не завершённые)
            var activeFlights = await _context.Flights
                .Where(f => f.status != "Завершен")
                .ToListAsync(); // Здесь добавляем фильтрацию по статусу

            // Создаем единый список объектов для представления
            var combinedData = new List<(string RouteInfo, string FlightInfo, TimeOnly DepartureTime)>();

            foreach (var route in routes)
            {
                foreach (var flight in activeFlights.Where(f => f.RouteId == route.id)) // Используем только активные рейсы
                {
                    combinedData.Add((
                        $"{route.cityDeparture.nameStop} → {route.cityArrival.nameStop}",
                        flight.Id.ToString(),
                        flight.timeDeparture 
                    ));
                }
            }

            return View(combinedData); // Передача данных в представление
        }

        [Authorize(Roles = "dispatcher")]
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
        [Authorize(Roles = "dispatcher")]
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
        [Authorize(Roles = "dispatcher")]
        public async Task<IActionResult> PrintTicket(int ticketId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Flight)
                    .ThenInclude(f => f.Route)
                        .ThenInclude(r => r.cityDeparture)
                .Include(t => t.Flight)
                    .ThenInclude(f => f.Route)
                        .ThenInclude(r => r.cityArrival)
                .FirstOrDefaultAsync(t => t.id == ticketId);

            if (ticket == null)
            {
                return NotFound();
            }

            return View(ticket);
        }

        [HttpGet]
        [Authorize(Roles = "dispatcher")]
        public IActionResult ReturnTicket()
        {

            return View();
        }

        // Метод поиска билетов для возврата
        [HttpPost]
        [Authorize(Roles = "dispatcher")]
        public async Task<IActionResult> ReturnTicket(string from, string to, TimeOnly departureTime)
        {
            // Параметры поиска
            var start = departureTime;
            var end = departureTime.AddMinutes(1);

            // Запрашиваем билеты

            var tickets = await _context.Tickets
                .Include(t => t.Flight)
                    .ThenInclude(f => f.Route)
                        .ThenInclude(r => r.cityDeparture)
                .Include(t => t.Flight)
                    .ThenInclude(f => f.Route)
                        .ThenInclude(r => r.cityArrival)
                .Where(t => t.Flight.Route.cityDeparture.nameStop == from &&
                            t.Flight.Route.cityArrival.nameStop == to &&
                            t.Flight.timeDeparture >= start &&
                            t.Flight.timeDeparture < end)
                .ToListAsync();


            if (!tickets.Any())
            {
                return View(new List<Tickets>()); // Передача пустой коллекции, чтобы показать отсутствие билетов
            }

            return View(tickets); // Возвращаем найденные билеты
        }


        // Показ страницы подтверждения возврата
        [HttpGet]
        [Authorize(Roles = "dispatcher")]
        public async Task<IActionResult> ConfirmReturn(int id)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Flight)
                    .ThenInclude(f => f.Route)
                        .ThenInclude(r => r.cityDeparture)
                .Include(t => t.Flight)
                    .ThenInclude(f => f.Route)
                        .ThenInclude(r => r.cityArrival)
                .FirstOrDefaultAsync(t => t.id == id);

            if (ticket == null || ticket.Flight == null)
            {
                return NotFound(); // Нет такого билета
            }
            return View(ticket); // Рендерим страницу подтверждения
        }

        // Подтверждение возврата билета
        [HttpPost]
        [Authorize(Roles = "dispatcher")]
        public async Task<IActionResult> PostReturnTicket(int ticketId)
        {
            var ticket = await _context.Tickets.FindAsync(ticketId);
            if (ticket == null)
            {
                return NotFound(); // Билет не найден
            }

            // Удаляем билет из базы данных
            _context.Remove(ticket);
            await _context.SaveChangesAsync();

            // Генерируем уведомление и переходим на главную страницу диспетчера
            TempData["SuccessMessage"] = "Возврат успешен!";
            return RedirectToAction("DispatcherDashboard", "Account"); // Переход на домашнюю страницу диспетчера
        }
    }
}
