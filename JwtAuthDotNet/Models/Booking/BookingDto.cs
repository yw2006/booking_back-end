using JwtAuthDotNet.Enums;
using JwtAuthDotNet.Models.Hotel;
using JwtAuthDotNet.Models.Room;

namespace JwtAuthDotNet.Models.Booking
{
    public class BookingDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid HotelId { get; set; }
        public Guid RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Nights { get; set; }
        public decimal TotalPrice { get; set; }
        public BookingStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        // Navigation DTOs
        public HotelDto? Hotel { get; set; }
        public RoomDto? Room { get; set; }
    }
}
