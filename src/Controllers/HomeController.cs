using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using HotelManager.Models;
using HotelManager.Data; // 1. Added this to access the database

namespace HotelManager.Controllers;

public class HomeController : Controller
{
    // 2. Create a private field for the database
    private readonly ApplicationDbContext _context;

    // 3. Inject the database when the HomeController boots up
    public HomeController(ApplicationDbContext context)
    {
        _context = context;
    }

    public IActionResult Index()
    {
        // 4. Fetch ONLY rooms where IsAvailable is true
        var availableRooms = _context.Rooms.Where(r => r.IsAvailable == true).ToList();
        
        // 5. Hand the available rooms to the homepage view
        return View(availableRooms);
    }
    public IActionResult NotDeveloped()
    {
        return View();
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}