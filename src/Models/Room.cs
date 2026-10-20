using System.ComponentModel.DataAnnotations;

namespace HotelManager.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string? RoomNumber { get; set;}

        public string? BedType { get; set;}

        public decimal PricePerNight { get; set; }

        public bool IsAvailable { get; set;}

    }
}