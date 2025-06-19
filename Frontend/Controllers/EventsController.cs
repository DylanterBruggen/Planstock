using Microsoft.AspNetCore.Mvc;

namespace Frontend.Controllers
{
    public class EventsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
