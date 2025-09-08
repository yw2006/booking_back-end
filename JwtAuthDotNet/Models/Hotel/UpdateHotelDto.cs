namespace JwtAuthDotNet.Models.Hotel
{
    public class UpdateHotelDto
    {
        public string? Name { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
    }
}

