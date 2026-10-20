using Microsoft.AspNetCore.Mvc;
using HotelManager.Models;
using HotelManager.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace HotelManager.Controllers
{
    public class RoomController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RoomController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. READ: Shows the table of all rooms
        [HttpGet]
        public IActionResult Index()
        {
            var allRooms = _context.Rooms.ToList();
            return View(allRooms);
        }

        // 2. CREATE (GET): Hands the blank form to the admin
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // 3. CREATE (POST): Receives the filled-out form and saves it
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Room newRoom)
        {
            bool roomExists = _context.Rooms.Any(r => r.RoomNumber == newRoom.RoomNumber);
            if (roomExists)
            {
                // If it exists, manually trigger a validation error on the RoomNumber field
                ModelState.AddModelError("RoomNumber", "This Room Number already exists in the database.");
            }
            if (ModelState.IsValid)
            {
                _context.Rooms.Add(newRoom);
                _context.SaveChanges();

                // Bonus fix: Redirects to the "Index" page of the "Room" controller!
                return RedirectToAction("Index", "Room");
            }
            return View(newRoom);
        }
        // 4. UPDATE (GET): The receptionist fetches the existing file and hands it to the admin
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            // If someone just types /Room/Edit without an ID, tell them we can't find it
            if (id == null)
            {
                return NotFound();
            }

            // Go to the database filing cabinet and find the room with this exact ID
            var room = _context.Rooms.Find(id);

            // If the room doesn't exist, return a 404 error
            if (room == null)
            {
                return NotFound();
            }

            // Hand the pre-filled form to the admin
            return View(room);
        }

        // 5. UPDATE (POST): The receptionist takes the edited form and overwrites the old file
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Room updatedRoom)
        {
            // Security check: Make sure a hacker didn't swap the ID in the URL while submitting the form
            if (id != updatedRoom.Id)
            {
                return NotFound();
            }
            //See if the room number belongs to a DIFFERENT
            bool roomExists = _context.Rooms.Any(r => r.RoomNumber == updatedRoom.RoomNumber && r.Id != updatedRoom.Id);
            if (roomExists)
            {
                ModelState.AddModelError("RoomNumber", "This Room Number is already taken by another room.");
            }

            if (ModelState.IsValid)
            {
                // Tell Entity Framework to overwrite the old data with the new data
                _context.Rooms.Update(updatedRoom);
                _context.SaveChanges();
                
                // Send them back to the Dashboard
                return RedirectToAction("Index", "Room");
            }

            // If they made a mistake (like erasing the room number), hand the form back with errors
            return View(updatedRoom);
        }
        // 6. DELETE (GET): The receptionist fetches the room file and asks for confirmation
        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Find the exact room in the database using the ID
            var room = _context.Rooms.Find(id);

            if (room == null)
            {
                return NotFound();
            }

            // Hand the room details to the confirmation page
            return View(room);
        }

        // 7. DELETE (POST): The admin confirms, and the receptionist permanently shreds the file
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var room = _context.Rooms.Find(id);
            
            if (room != null)
            {
                // Tell Entity Framework to remove this room from the database
                _context.Rooms.Remove(room);
                _context.SaveChanges(); 
            }
            
            // Send the user back to the Dashboard
            return RedirectToAction("Index", "Room");
        }
    }
}