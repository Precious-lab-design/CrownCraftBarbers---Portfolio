using System.Diagnostics;
using CrownCraftBarbers.Models;
using Microsoft.AspNetCore.Mvc;

namespace CrownCraftBarbers.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}