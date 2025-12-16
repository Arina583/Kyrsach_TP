using Company.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        [Authorize(Roles = "logist")]
        public async Task<IActionResult> Create()
        {
            ViewData["Routes"] = await _context.Routes.Include(r => r.cityDeparture).Include(r => r.cityArrival).ToListAsync();
            ViewData["Buses"] = await _context.Buses.ToListAsync();
            ViewData["Drivers"] = await _context.Drivers.ToListAsync();
            return View();
        }

        [Authorize(Roles = "logist")]
        public async Task<IActionResult> Create(Flights flight)
        {
            if (ModelState.IsValid)
            {
                _context.Add(flight);
                await _context.SaveChangesAsync();
                return RedirectToAction("Schedule");
            }

            return View(flight);
        }
    }
}