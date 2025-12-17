using Company.Models;
using Company.ViewsModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Company.Controllers
{
    public class DispatcherController : Controller
    {
        private readonly DbClassContext _context;

        public DispatcherController(DbClassContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        // GET: /Dispatcher/Sale
        public IActionResult Sale()
        {
            var model = new SaleViewModels();
            model.AvailableFlights = GetAvailableFlights(); // метод возвращает список рейсов
            return View(model);
        }

        // POST: /Dispatcher/Sale
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Sale(SaleViewModels viewModel)
        {
            if (!ModelState.IsValid)
            {
                viewModel.AvailableFlights = GetAvailableFlights();
                return View(viewModel); // Возвращаем обратно с ошибками
            }

            // Создаем новый объект пассажира
            var passenger = new Passengers
            {
                firstName = viewModel.Passenger.firstName,
                lastName = viewModel.Passenger.lastName,
                patronymic = viewModel.Passenger.patronymic,
                passportData = viewModel.Passenger.passportData,
                email = viewModel.Passenger.email
            };

            await _context.Passengers.AddAsync(passenger);
            await _context.SaveChangesAsync();

            // Добавляем билет и связываем с пассажиром и рейсом
            var ticket = new Tickets
            {
                numberSeat = GenerateRandomNumber(), // Метод генерирует случайный номер места
                status = "Куплен",
                FlightId = viewModel.Flight.Id,
                PassengerId = passenger.id
            };

            await _context.Tickets.AddAsync(ticket);
            await _context.SaveChangesAsync();

            // Переадресация на страницу успешного завершения
            return RedirectToAction(nameof(Success));
        }

        // Страница успешной покупки
        public IActionResult Success()
        {
            return View();
        }

        // Вспомогательные методы
        private IEnumerable<SelectListItem> GetAvailableFlights()
        {
            // Логика получения списка доступных рейсов
            var flights = _context.Flights.ToList().Select(f =>
                new SelectListItem { Value = f.Id.ToString(), Text = $"{f.timeArrival}, {f.timeDeparture}" });
            return flights;
        }

        private static Random random = new Random();
        private int GenerateRandomNumber() => random.Next(1, 39);

        [Authorize(Roles = "dispatcher")] // Защищаем метод авторизацией по роли
        public IActionResult ReturnTicket()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "dispatcher")] // Защищаем метод авторизацией по роли
        public IActionResult PassportDate()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "dispatcher")] // Защищаем метод авторизацией по роли
        public IActionResult Payment()
        {
            return View();
        }
    }
}
