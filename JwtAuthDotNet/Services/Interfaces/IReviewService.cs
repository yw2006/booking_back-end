using JwtAuthDotNet.Models.Review;

namespace JwtAuthDotNet.Services.Interfaces
{
    public interface IReviewService
    {
        Task<List<ReviewDto>> GetHotelReviews(Guid hotelId);
        Task<ReviewDto?> GetReview(Guid id);
        Task<(bool Success, string Message, Guid? ReviewId)> CreateReview(CreateReviewDto dto, Guid userId, Guid hotelId);
        Task<(bool Success, string Message)> UpdateReview(Guid reviewid, UpdateReviewDto dto, Guid userId);
        Task<(bool Success, string Message)> DeleteReview(Guid reviewid, Guid userId);
    }
}
