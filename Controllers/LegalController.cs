using Microsoft.AspNetCore.Mvc;

namespace CrownCraftBarbers.Controllers
{
    public class LegalController : Controller
    {
        [HttpGet]
        public IActionResult Terms()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Privacy()
        {
            return View();
        }
    }
}