using Microsoft.AspNetCore.Mvc;

namespace HotelManager.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}