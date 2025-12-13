using Company.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Company.Controllers
{
    public class BusController : Controller
    {
        private readonly DbClassContext _context;

        public BusController(DbClassContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // Добавление водителя
        [Authorize(Roles = "logist")]
        [HttpGet]
        public IActionResult AddBus()
        {
            return View();
        }

        [Authorize(Roles = "logist")]
        [HttpPost]
        public IActionResult AddBus(Buses buses)
        {
            if (ModelState.IsValid) // Проверка на валидность модели
            {
                _context.Buses.Add(buses); // Добавляем водителя в DbSet
                _context.SaveChanges(); // Сохраняем изменения в базе данных
                return RedirectToAction("Buses");
            }
            return View(buses); // Если модель невалидна, возвращаем представление с моделью
        }

        // Редактирование водителя
        [Authorize(Roles = "logist")]
        [HttpGet]
        public IActionResult EditBus(int id)
        {
            Buses buses = _context.Buses.Find(id); // Ищем водителя в базе данных по id
            if (buses == null)
            {
                return NotFound();
            }
            return View(buses);
        }

        [Authorize(Roles = "logist")]
        [HttpPost]
        public IActionResult EditBus(Buses buses)
        {
            if (ModelState.IsValid)
            {
                _context.Buses.Update(buses);
                _context.SaveChanges();
                return RedirectToAction("Buses");
            }
            return View(buses);
        }

        //Удаление водителя
        [Authorize(Roles = "logist")]
        public IActionResult DeleteBus(int id)
        {
            Buses buses = _context.Buses.Find(id);
            if (buses == null)
            {
                return NotFound();
            }
            return View(buses); // Отображаем представление для подтверждения удаления
        }

        [Authorize(Roles = "logist")]
        [HttpPost, ActionName("DeleteBus")]// подтверждение удаления
        public IActionResult DeleteConfirmed(int id)
        {
            Buses buses = _context.Buses.Find(id);
            if (buses == null)
            {
                return NotFound();
            }

            _context.Buses.Remove(buses);
            _context.SaveChanges();
            return RedirectToAction("Buses");
        }
    }
}
