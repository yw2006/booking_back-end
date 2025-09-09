namespace JwtAuthDotNet.Models.Review;

public class UpdateReviewDto
{
    public Guid? HotelId { get; set; }
    public int? Rating { get; set; }
    public string? Comment { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
