using System.ComponentModel.DataAnnotations;

namespace JwtAuthDotNet.Entities
{
    public class Review
    {
        public Guid Id { get; set; }

        [Required]
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        [Required]
        public Guid HotelId { get; set; }
        public Hotel Hotel { get; set; } = null!;

        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
