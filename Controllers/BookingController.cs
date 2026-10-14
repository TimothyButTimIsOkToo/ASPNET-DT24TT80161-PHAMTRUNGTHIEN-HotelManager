using Microsoft.AspNetCore.Mvc;
using HotelManager.Data;
using HotelManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // 1. Added this for security

namespace HotelManager.Controllers;

[Authorize] // 2. This locks the entire controller so only logged-in users can book
public class BookingController : Controller
{
    private readonly ApplicationDbContext _context;

    public BookingController(ApplicationDbContext context)
    {
        _context = context;
    }

    // READ: Shows the table of all reservations to the admin
    [Authorize(Roles = "Admin")] // 3. This ensures ONLY the Admin can see everyone's bookings
    [HttpGet]
    public IActionResult Index()
    {
        // Fetch all bookings and attach the related Room data
        var allBookings = _context.Bookings.Include(b => b.Room).ToList();
        return View(allBookings);
    }

    // 4. HERE IS THE NEW METHOD: Shows a client their personal reservations
    [HttpGet]
    public IActionResult MyBookings()
    {
        // Get the email of the person currently looking at the screen
        var currentUserEmail = User.Identity?.Name;
        
        // Fetch ONLY the bookings that match their email
        var myBookings = _context.Bookings
            .Include(b => b.Room)
            .Where(b => b.ClientEmail == currentUserEmail)
            .ToList();
            
        return View(myBookings);
    }

    // 1. CREATE (GET): Loads the checkout form when the customer clicks "Book This Room"
    [HttpGet]
    public IActionResult Create(int roomId)
    {
        // Find the specific room they clicked on
        var room = _context.Rooms.Find(roomId);
        
        // If the room doesn't exist or is already booked, stop them
        if (room == null || room.IsAvailable == false)
        {
            return NotFound();
        }
        // ... Keep the rest of your Create GET and POST methods exactly as they are below this point! ...
            //Create the booking and pre-fill today
            var booking = new Booking 
            { 
                RoomId = roomId,
                CheckInDate = DateTime.Today,
                CheckOutDate = DateTime.Today.AddDays(1)
            };
            
            // Use ViewBag to temporarily pass the room details to the HTML page so the customer can see what they are buying
            ViewBag.RoomNumber = room.RoomNumber;
            ViewBag.PricePerNight = room.PricePerNight;

            return View(booking);
        }
        // 2. CREATE (POST): Processes the checkout form
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Booking newBooking)
        {
            // Define the allowed booking window (ignoring the specific time of day)
        DateTime today = DateTime.Today;
        DateTime maxAdvanceDate = today.AddMonths(3);

        // 1. Check-In must be today or later (This automatically blocks 01/01/0001)
        if (newBooking.CheckInDate.Date < today)
        {
            ModelState.AddModelError("CheckInDate", "Check-In date cannot be in the past.");
        }

        // 2. Check-In cannot be more than 3 months in the future
        if (newBooking.CheckInDate.Date > maxAdvanceDate)
        {
            ModelState.AddModelError("CheckInDate", "You can only book up to 3 months in advance.");
        }

        // 3. Check-Out must be strictly after Check-In
        if (newBooking.CheckOutDate.Date <= newBooking.CheckInDate.Date)
        {
            ModelState.AddModelError("CheckOutDate", "Check-Out date must be at least one day after Check-In.");
        }
        
        if (ModelState.IsValid)
        {
            // 1. Go to the database and find the specific room they are trying to book
            var room = _context.Rooms.Find(newBooking.RoomId);

            //attach the logged-in user's email to this booking!
            newBooking.ClientEmail = User.Identity?.Name;

            if (room != null)
            {
                // 2. Change the room's status to Booked
                room.IsAvailable = false;
                _context.Rooms.Update(room);

                // 3. Save the new booking record
                _context.Bookings.Add(newBooking);
                
                // 4. Commit all changes to SQL Express at the same time
                _context.SaveChanges();

                // 5. Send them back to the Home page
                return RedirectToAction("Index", "Home");
            }
        }

        // If they missed a field (like their name), we need to reload the form
        // We also must fetch the ViewBag data again so the blue info box doesn't break
        var originalRoom = _context.Rooms.Find(newBooking.RoomId);
        if (originalRoom != null)
        {
            ViewBag.RoomNumber = originalRoom.RoomNumber;
            ViewBag.PricePerNight = originalRoom.PricePerNight;
        }
        
        return View(newBooking);
    }
}