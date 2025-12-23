using Company.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
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

        public async Task<IActionResult> BuyTicket(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var flight = await _context.Flights
                .Include(f => f.Route)
                    .ThenInclude(r => r.cityDeparture)
                .Include(f => f.Route)
                    .ThenInclude(r => r.cityArrival)
                .Include(f => f.Bus)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (flight == null)
            {
                return NotFound();
            }

            // Получаем список забронированных мест
            var reservedSeats = await _context.Tickets
                .Where(t => t.FlightId == id && (t.status == "Забронирован" || t.status == "Куплен"))
                .Select(t => t.numberSeat)
                .ToListAsync();

            ViewBag.ReservedSeats = reservedSeats; // Передаем список мест в представление
            ViewBag.NumberOfSeats = flight.Bus.numberSeat; // Передаем количество мест

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
                return BadRequest("В настоящее время это место недоступно.");
            }

            if (actionType == "buy")
            {
                // Получаем маршрут и проверяем длину
                var route = await _context.Routes.FindAsync(flight.RouteId);

                var departureStopId = route.DepartureStopsId;

                // Сохраняем DepartureStopsId в TempData
                TempData["DepartureStopsId"] = departureStopId;
                if (route == null)
                {
                    return BadRequest("Маршрут для этого рейса не найден.");
                }

                if (route.length > 30)
                {
                    // Перенаправляем на страницу ввода паспортных данных
                    return RedirectToAction("PassportData", new { id = id, seatNumber = seatNumber, email = email });
                }
                else
                {
                    // Перенаправляем на страницу оплаты
                    return RedirectToAction("Payment", new { id = id, seatNumber = seatNumber, email = email });
                }
            }

            if (actionType == "reserve")
            {
                // Получаем маршрут связанный с рейсом
                var route = await _context.Routes.FindAsync(flight.RouteId);
                if (route == null)
                {
                    return BadRequest("Маршрут для этого рейса не найден.");
                }

                var departureStopId = route.DepartureStopsId;

                // Сохраняем DepartureStopsId в TempData
                TempData["DepartureStopsId"] = departureStopId;

                var ticket = new Tickets
                {
                    FlightId = id,
                    numberSeat = seatNumber,
                    status = "Забронирован",
                    PassengerId = 1, // Вставляем id универсального пассажира
                    StopId = departureStopId
                };

                _context.Tickets.Add(ticket);
                await _context.SaveChangesAsync();

                /*// Удаление брони через 30 минут
                Task.Delay(TimeSpan.FromMinutes(30)).ContinueWith(async _ =>
                {
                    var ticketToDelete = await _context.Tickets.FindAsync(ticket.id);
                    if (ticketToDelete != null && ticketToDelete.status == "Забронирован")
                    {
                        _context.Tickets.Remove(ticketToDelete);
                        await _context.SaveChangesAsync();
                    }
                });*/

                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        // Методы для отображения страниц ввода данных и оплаты
        [HttpGet]
        public IActionResult PassportData(int id, int seatNumber, string email)
        {
            // Передаем параметры в представление, чтобы их можно было использовать при отправке формы
            ViewBag.Id = id;
            ViewBag.SeatNumber = seatNumber;
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> PassportData(string passport_series, string passport_number, DateTime date_of_birth, int id, int seatNumber, string email)
        {
            //Создаем строку passportData
            string passportData = $"{passport_series} {passport_number} {date_of_birth.ToShortDateString()}";

            // Создаем и сохраняем пассажира с паспортными данными
            var passenger = new Passengers
            {
                firstName = " ",
                lastName = " ",
                patronymic = " ",
                passportData = passportData,
                email = " "
            };
            _context.Passengers.Add(passenger);
            await _context.SaveChangesAsync();

            // Перенаправляем на оплату, передавая Id пассажира и остальные параметры
            return RedirectToAction("Payment", new { passengerId = passenger.id, id = id, seatNumber = seatNumber, passportData = passportData, email = email});
        }


        [HttpGet]
        public IActionResult Payment(int id, int seatNumber, string passportData, string email, int? passengerId = null)
        {
            // Передаем параметры в представление
            ViewBag.Id = id;
            ViewBag.SeatNumber = seatNumber;
            ViewBag.PassengerId = passengerId; // Может быть null, если переход был без ввода паспортных данных
            ViewBag.PassportData = passportData;
            ViewBag.email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Payment(string lastName, string firstName, string patronymic, int id, int seatNumber, string passportData, string email, int? passengerId = null)
        {
            Passengers passenger;
            // Если есть PassengerId, значит, паспортные данные уже введены. Иначе, создаем нового пассажира
            if (passengerId.HasValue)
            {
                passenger = await _context.Passengers.FindAsync(passengerId);
                passenger.firstName = firstName;
                passenger.lastName = lastName;
                passenger.patronymic = patronymic;
                passenger.passportData = passportData;
                passenger.email = email;
                if (passenger == null) return NotFound();
            }
            else
            {
                passenger = new Passengers();
                _context.Passengers.Add(passenger);
                passenger.firstName = firstName;
                passenger.lastName = lastName;
                passenger.patronymic = patronymic;
                passenger.passportData = "";
                passenger.email = email;
            }

            // сохраняем изменения
            await _context.SaveChangesAsync();

            // Здесь код для обработки оплаты
            // ...

            // После успешной добавляем купленный билет в бд
            int departureStopId = (int)TempData["DepartureStopsId"];

            var ticket = new Tickets
            {
                FlightId = id,
                numberSeat = seatNumber,
                status = "Куплен",
                PassengerId = passenger.id,
                StopId = departureStopId,
            };

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            //После успешной покупки
            return RedirectToAction("Index", "Home");
        }
    }
}
