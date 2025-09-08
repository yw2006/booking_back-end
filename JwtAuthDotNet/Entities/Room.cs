using JwtAuthDotNet.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JwtAuthDotNet.Entities
{
    public class Room
    {
        public Guid Id { get; set; }

        [Required]
        public Guid HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;

        [Required]
        public RoomTypeName Name { get; set; }

        [NotMapped]
        public int Capacity => Name switch
        {
            RoomTypeName.Deluxe => 2,
            RoomTypeName.Suite => 4,
            RoomTypeName.Standard => 2,
            RoomTypeName.Family => 5,
            RoomTypeName.Single => 1,
            RoomTypeName.Double => 2,
            _ => 1
        };
        [Column(TypeName = "decimal(18,2)")]
        public decimal BasePrice { get; set; }
        public string? Description { get; set; }

        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }

}
