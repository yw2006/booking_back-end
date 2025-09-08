namespace JwtAuthDotNet.Models.Review;

public class CreateReviewDto
{
    public Guid UserId { get; set; }
    public Guid HotelId { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
}
