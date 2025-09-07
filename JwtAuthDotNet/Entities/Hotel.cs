using System.ComponentModel.DataAnnotations;

namespace JwtAuthDotNet.Entities
{
    public class Hotel
    {
        public Guid Id { get; set; }

        [Required, MaxLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required, MaxLength(300)]
        public string Address { get; set; } = string.Empty;

        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation
        public ICollection<Room> RoomTypes { get; set; } = new List<Room>();
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
