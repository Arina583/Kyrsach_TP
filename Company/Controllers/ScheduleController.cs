using Company.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Threading.Tasks;

namespace Company.Controllers
{
    public class ScheduleController : Controller
    {
        private readonly DbClassContext _context;

        public ScheduleController(DbClassContext context)
        {
            _context = context;
        }

        // GET: /Logist/CreateSchedule
        public IActionResult CreateSchedule()
        {
            var drivers = _context.Drivers.ToList();
            var buses = _context.Buses.ToList();
            var routes = _context.Routes.Include(r => r.cityDeparture).Include(r => r.cityArrival).ToList();

            ViewBag.Drivers = drivers;
            ViewBag.Buses = buses;
            ViewBag.Routes = routes;

            return View(); // Отображаем пустую форму
        }

        // POST: /Logist/CreateSchedule
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSchedule(Flights model)
        {
            if (!ModelState.IsValid)
            {
                return View(model); // Если форма заполнена неверно, возвращаемся обратно
            }

            try
            {
                await _context.Flights.AddAsync(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Schedule", "Logist"); // Перенаправляем на список рейсов
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка при создании рейса: {ex.Message}";
                return View(model); // Возвращаемся назад с сообщением об ошибке
            }
        }

        // GET: EditSchedule/{id}
        public async Task<IActionResult> EditSchedule(int id)
        {
            // Поиск рейса по идентификатору
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound(); // Рейс не найден
            }

            // Подгружаем данные для форм
            var routes = await _context.Routes.Include(r => r.cityDeparture).Include(r => r.cityArrival).ToListAsync();
            var buses = await _context.Buses.ToListAsync();
            var drivers = await _context.Drivers.ToListAsync();

            if (routes == null || buses == null || drivers == null)
            {
                throw new InvalidOperationException("Ошибка загрузки данных из базы");
            }

            // Формируем ViewBag с нужными объектами
            ViewBag.Routes = routes;
            ViewBag.Buses = buses;
            ViewBag.Drivers = drivers;

            return View(flight); // Отправляем рейс в представление
        }

        // POST: EditSchedule/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditSchedule([FromForm] Flights updatedFlight)
        {
            if (!ModelState.IsValid)
            {
                return View(updatedFlight); // Возврат к форме с ошибками
            }

            try
            {
                // Найти существующий рейс в базе данных
                var existingFlight = await _context.Flights.FindAsync(updatedFlight.Id);
                if (existingFlight != null)
                {
                    // Обновляем поля существующей записи
                    existingFlight.numberFlight = updatedFlight.numberFlight;
                    existingFlight.timeDeparture = updatedFlight.timeDeparture;
                    existingFlight.timeArrival = updatedFlight.timeArrival;
                    existingFlight.status = updatedFlight.status;
                    existingFlight.RouteId = updatedFlight.RouteId;
                    existingFlight.BusId = updatedFlight.BusId;
                    existingFlight.DriverId = updatedFlight.DriverId;

                    // Сохраняем изменения
                    await _context.SaveChangesAsync();
                    return RedirectToAction("schedule", "Logist"); // Переход на главную страницу или список рейсов
                }
                else
                {
                    return NotFound(); // Не нашли рейс
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка при обновлении рейса: {ex.Message}";
                return View(updatedFlight); // Возвращаемся обратно с сообщением об ошибке
            }
        }

        // GET: DeleteSchedule/id
        public async Task<IActionResult> DeleteSchedule(int id)
        {
            // Нахождение рейса по идентификатору
            var flight = await _context.Flights.FindAsync(id);
            if (flight == null)
            {
                return NotFound(); // Рейс не найден
            }

            return View(flight); // Перенаправляет на подтверждение удаления
        }

        // POST: DeleteSchedule/id
        [HttpPost, ActionName("DeleteSchedule")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmDelete(int id)
        {
            var flight = await _context.Flights.FindAsync(id);
            if (flight != null)
            {
                _context.Flights.Remove(flight);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Schedule", "Logist"); // Вернёмся к списку рейсов
        }
    }
}