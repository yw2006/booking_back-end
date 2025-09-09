using JwtAuthDotNet.Enums;

namespace JwtAuthDotNet.Models.HotelSearch
{
    public class AvailableRoomDto
    {
        public Guid Id { get; set; }
        public Guid HotelId { get; set; }
        public RoomTypeName Name { get; set; }
        public int Capacity { get; set; }
        public decimal BasePrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string? Description { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime Date { get; set; }
    }
}
