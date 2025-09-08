using JwtAuthDotNet.Enums;

namespace JwtAuthDotNet.Models.Room
{
    public class UpdateRoomDto
    {
        public Guid? HotelId { get; set; }
        public RoomTypeName? Name { get; set; }
        public decimal? BasePrice { get; set; }
        public string? Description { get; set; }
        public bool? IsAvailable { get; set; } = true;
    }
}
