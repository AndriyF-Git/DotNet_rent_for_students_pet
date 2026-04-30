using Microsoft.AspNetCore.Mvc;

namespace rent_for_students.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            // Можна редірект на Listings/Index
            return RedirectToAction("Index", "Listings");
        }
    }
}
