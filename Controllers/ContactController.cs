using Microsoft.AspNetCore.Mvc;

namespace CrownCraftBarbers.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
