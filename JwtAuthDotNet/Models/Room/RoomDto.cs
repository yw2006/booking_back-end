using JwtAuthDotNet.Entities;
using JwtAuthDotNet.Enums;

namespace JwtAuthDotNet.Models.Room;

public class RoomDto
{
    public Guid Id { get; set; }
    public Guid HotelId { get; set; }
    public RoomTypeName Name { get; set; }

    public int Capacity => Name switch
    {
        RoomTypeName.Deluxe => 2,
        RoomTypeName.Suite => 4,
        RoomTypeName.Standard => 2,
        RoomTypeName.Family => 5,
        RoomTypeName.Single => 1,
        RoomTypeName.Double => 2,
        _ => 1
    };

    public decimal BasePrice { get; set; }
    public string? Description { get; set; }
    public bool IsAvailable { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}
