using Microsoft.AspNetCore.Mvc;

namespace Company.Controllers
{
    public class ScheduleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
