using JwtAuthDotNet.Models.Room;

namespace JwtAuthDotNet.Models.Hotel
{
    public class HotelDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public double AverageRating { get; set; }
        public int ReviewCount { get; set; }
        public ICollection<RoomDto> Rooms { get; set; } = new List<RoomDto>();

    }
}

