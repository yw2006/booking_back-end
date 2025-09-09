namespace JwtAuthDotNet.Models.Booking
{
    public class SavingBookingDto
    {
        public Guid UserId { get; set; }
        public Guid RoomId { get; set; }
        public DateTime CheckIn { get; set; }
        public int Nights { get; set; }


    }
}
