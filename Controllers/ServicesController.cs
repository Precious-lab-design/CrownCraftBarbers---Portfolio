using CrownCraftBarbers.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrownCraftBarbers.Controllers
{
    public class ServicesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ServicesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var services = await _context.Services
                .ToListAsync();

            services = services
                .OrderBy(s => s.Price)
                .ToList();

            return View(services);
        }
    }
}