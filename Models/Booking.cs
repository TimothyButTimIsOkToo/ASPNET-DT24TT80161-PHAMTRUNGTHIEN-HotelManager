using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManager.Models
{
    public class Booking
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Please enter your full name.")]
        public string CustomerName { get; set; } = string.Empty; // Added "= string.Empty;"
        // Links booking to logged in user account
        public string? ClientEmail { get; set; }
        [Required]
        public DateTime CheckInDate { get; set; }
        [Required]
        public DateTime CheckOutDate { get; set; }
        // Foreign Key - links booking to a specific Room in the database!
        [Required]
        public int RoomId { get; set; }
        // Entity Framework how the two tables are physically connected
        [ForeignKey("RoomId")]
        public Room? Room { get; set; }
    }
}