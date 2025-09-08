namespace JwtAuthDotNet.Models.Review;

public class ReviewDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid HotelId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
