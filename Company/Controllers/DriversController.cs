using Company.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Company.Controllers
{
    public class DriversController : Controller
    {

        private readonly DbClassContext _context;

        public DriversController(DbClassContext context)
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
        public IActionResult AddDriver()
        {
            return View();
        }

        [Authorize(Roles = "logist")]
        [HttpPost]
        public IActionResult AddDriver(Drivers driver)
        {
            if (ModelState.IsValid) // Проверка на валидность модели
            {
                _context.Drivers.Add(driver); // Добавляем водителя в DbSet
                _context.SaveChanges(); // Сохраняем изменения в базе данных
                return RedirectToAction("Drivers");
            }
            return View(driver); // Если модель невалидна, возвращаем представление с моделью
        }

        // Редактирование водителя
        [Authorize(Roles = "logist")]
        [HttpGet]
        public IActionResult EditDriver(int id)
        {
            Drivers driver = _context.Drivers.Find(id); // Ищем водителя в базе данных по id
            if (driver == null)
            {
                return NotFound();
            }
            return View(driver);
        }

        [Authorize(Roles = "logist")]
        [HttpPost]
        public IActionResult EditDriver(Drivers driver)
        {
            if (ModelState.IsValid)
            {
                _context.Drivers.Update(driver);
                _context.SaveChanges();
                return RedirectToAction("Drivers");
            }
            return View(driver);
        }

        //Удаление водителя
        [Authorize(Roles = "logist")]
        public IActionResult DeleteDriver(int id)
        {
            Drivers driver = _context.Drivers.Find(id);
            if (driver == null)
            {
                return NotFound();
            }
            return View(driver); // Отображаем представление для подтверждения удаления
        }

        [Authorize(Roles = "logist")]
        [HttpPost, ActionName("DeleteDriver")]// подтверждение удаления
        public IActionResult DeleteConfirmed(int id)
        {
            Drivers driver = _context.Drivers.Find(id);
            if (driver == null)
            {
                return NotFound();
            }

            _context.Drivers.Remove(driver);
            _context.SaveChanges();
            return RedirectToAction("Drivers");
        }
    }
}
