namespace JwtAuthDotNet.Models.Booking
{
    public class CreateBookingDto
    {
        public Guid UserId { get; set; }
        public Guid HotelId { get; set; }
        public Guid RoomTypeId { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }

    }
}
