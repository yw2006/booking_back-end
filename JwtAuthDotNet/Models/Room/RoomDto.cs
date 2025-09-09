using JwtAuthDotNet.Enums;

namespace JwtAuthDotNet.Models.Room
{
    public class RoomDto
    {
        public Guid Id { get; set; }
        public Guid HotelId { get; set; }
        public RoomTypeName Name { get; set; } 
        public decimal BasePrice { get; set; }
        public DateTime Date { get; set; }
        public string? Description { get; set; }
    }

    }
