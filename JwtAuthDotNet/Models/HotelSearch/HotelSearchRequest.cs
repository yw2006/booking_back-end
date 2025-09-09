using JwtAuthDotNet.Enums;
using System.ComponentModel.DataAnnotations;

namespace JwtAuthDotNet.Models.HotelSearch
{
    public class HotelSearchRequest
    {
        [Required(ErrorMessage = "City is required")]
        [MaxLength(100, ErrorMessage = "City name cannot exceed 100 characters")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Check-in date is required")]
        public DateTime CheckIn { get; set; }

        [Required(ErrorMessage = "Check-out date is required")]
        public DateTime CheckOut { get; set; }

        [Range(1, 10, ErrorMessage = "Number of guests must be between 1 and 10")]
        public int Guests { get; set; } = 1;

        [Range(0, 5000, ErrorMessage = "Minimum price must be between 0 and 5000")]
        public decimal? MinPrice { get; set; }

        [Range(0, 5000, ErrorMessage = "Maximum price must be between 0 and 5000")]
        public decimal? MaxPrice { get; set; }

        public List<RoomTypeName>? RoomTypes { get; set; }

        // Pagination
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
