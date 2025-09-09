namespace JwtAuthDotNet.Models.HotelSearch
{
    public class HotelSearchResultDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ThumbnailUrl { get; set; }
        public List<AvailableRoomDto> AvailableRooms { get; set; } = new();
        public decimal MinPrice { get; set; }
        public decimal MaxPrice { get; set; }
        public int TotalAvailableRooms { get; set; }
        public double? AverageRating { get; set; }
        public int ReviewCount { get; set; }
    }
}
