using JwtAuthDotNet.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace JwtAuthDotNet.Entities { public class Booking 
    { public Guid Id { get; set; } 
        [Required] public Guid UserId { get; set; } 
        public User User { get; set; } = null!; 
        [Required] public Guid RoomId { get; set; } 
        public Room Room { get; set; } = null!; 
        public DateTime CheckIn { get; set; } 
        public DateTime CheckOut { get; set; } 
        public int Nights { get; set; } 
        [Column(TypeName = "decimal(18,2)")] public decimal TotalPrice { get; set; } 
        [Required] public BookingStatus Status { get; set; } = BookingStatus.Pending;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow; } 
}