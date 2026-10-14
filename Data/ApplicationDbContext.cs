using HotelManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HotelManager.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
{
    //for Rooms
    public DbSet<Room> Rooms { get; set; }
    //for Bookings
    public DbSet<Booking> Bookings { get; set; }
}
