using Company.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        [Authorize(Roles = "logist")]
        public IActionResult Report()
        {
            // Получаем список городов отправления и назначения из базы данных
            var departureCities = _context.Stops.ToList();
            var destinationCities = _context.Stops.ToList(); // Или используйте другой набор данных, если нужно

            // Получаем список времени отправления рейсов из базы данных
            var departureTimes = _context.Flights.Select(f => f.timeDeparture).Distinct().ToList();


            // Создаем списки SelectListItem для передачи в представление
            ViewBag.DepartureCities = new SelectList(departureCities, "Id", "nameStop"); // "Id" - значение, "nameStop" - текст
            ViewBag.DestinationCities = new SelectList(destinationCities, "Id", "nameStop");
            ViewBag.DepartureTimes = new SelectList(departureTimes);


            return View();
        }

        [HttpPost]
        [Authorize(Roles = "logist")]
        public IActionResult GenerateReport(int departure, int destination, TimeOnly departureTime)
        {
            // 1. Получаем данные о рейсе, как и раньше
            var flight = _context.Flights
                .Include(f => f.Route)
                    .ThenInclude(r => r.cityDeparture)
                .Include(f => f.Route)
                    .ThenInclude(r => r.cityArrival)
                .FirstOrDefault(f => f.Route.cityDeparture.Id == departure &&
                                     f.Route.cityArrival.Id == destination &&
                                     f.timeDeparture == departureTime);

            if (flight == null)
            {
                return NotFound("Рейс не найден");
            }

            // 2. Получаем список купленных билетов для этого рейса
            var purchasedTickets = _context.Tickets
                .Where(t => t.FlightId == flight.Id && t.status == "Куплен")
                .ToList();

            // 3. Извлекаем номера мест из билетов
            var seatNumbers = purchasedTickets.Select(t => t.numberSeat).ToList();

            // 4. Формируем данные для отчета, включая номера мест
            var reportData = new
            {
                ReisInfo = $"Рейс: {flight.Route.cityDeparture.nameStop} - {flight.Route.cityArrival.nameStop}",
                Time = $"Время: {flight.timeDeparture} - {flight.timeArrival}",
                Route = $"Маршрут: {flight.Route.intermediateStops}",
                BiletCount = purchasedTickets.Count,
                SeatNumbers = string.Join(", ", seatNumbers)
            };

            // Возвращаем представление с данными для отчета
            return View("ReportView", reportData);
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

        // Метод для добавления нового автобуса
        public IActionResult AddBus()
        {
            return View();
        }

        // Метод для добавления нового автобуса
        [HttpPost]
        public IActionResult AddBus(Buses bus)
        {

            _context.Add(bus);
            _context.SaveChanges();
            return RedirectToAction("Buses");
        }

        // Метод для редактирования автобуса
        public IActionResult EditBus(int id)
        {
            Buses bus = _context.Buses.FirstOrDefault(b => b.id == id);
            if (bus == null)
            {
                return NotFound();
            }
            return View(bus);
        }

        // Метод для редактирования автобуса
        [HttpPost]
        public IActionResult EditBus(Buses bus)
        {
            Buses existingBus = _context.Buses.FirstOrDefault(b => b.id == bus.id);
            if (existingBus == null)
            {
                return NotFound();
            }

            existingBus.stateNumber = bus.stateNumber;
            existingBus.numberSeat = bus.numberSeat;
            existingBus.status = bus.status;

            _context.Update(existingBus);
            _context.SaveChanges();

            return RedirectToAction("Buses");
        }

        // Методы для удаления автобуса
        [HttpGet]
        public IActionResult DeleteBus(int id)
        {
            Buses bus = _context.Buses.FirstOrDefault(b => b.id == id);
            if (bus == null)
            {
                return NotFound();
            }
            return View(bus);
        }

        [HttpPost]
        public IActionResult DeleteBus(Buses buses)
        {
            _context.Remove(buses);
            _context.SaveChanges();
            return RedirectToAction("Buses");
        }

        // Метод для добавления нового водителя
        public IActionResult AddDriver()
        {
            return View();
        }

        // Метод для добавления нового водителя
        [HttpPost]
        public IActionResult AddDriver(Drivers driver)
        {

            _context.Add(driver);
            _context.SaveChanges();
            return RedirectToAction("Drivers");
        }

        // Метод для редактирования данный водителя
        public IActionResult EditDriver(int id)
        {
            Drivers driver = _context.Drivers.FirstOrDefault(b => b.id == id);
            if (driver == null)
            {
                return NotFound();
            }
            return View(driver);
        }

        // Метод для редактирования данных водителя
        [HttpPost]
        public IActionResult EditDriver(Drivers driver)
        {
            Drivers existingDriver = _context.Drivers.FirstOrDefault(d => d.id == driver.id);
            if (existingDriver == null)
            {
                return NotFound();
            }

            existingDriver.firstName = driver.firstName;
            existingDriver.lastName = driver.lastName;
            existingDriver.patronymic = driver.patronymic;
            existingDriver.driverLicense = driver.driverLicense;
            existingDriver.contactInformation = driver.contactInformation;
            existingDriver.status = driver.status;

            _context.Update(existingDriver);
            _context.SaveChanges();

            return RedirectToAction("Drivers");
        }

        // Методы для удаления водителя
        [HttpGet]
        public IActionResult DeleteDriver(int id)
        {
            Drivers driver = _context.Drivers.FirstOrDefault(b => b.id == id);
            if (driver == null)
            {
                return NotFound();
            }
            return View(driver);
        }

        [HttpPost]
        public IActionResult DeleteDriver(Drivers drivers)
        {
            _context.Remove(drivers);
            _context.SaveChanges();
            return RedirectToAction("Drivers");
        }
    }
}
