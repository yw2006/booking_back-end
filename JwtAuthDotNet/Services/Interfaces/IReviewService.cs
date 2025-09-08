using JwtAuthDotNet.Models.Review;

namespace JwtAuthDotNet.Services.Interfaces
{
    public interface IReviewsService
    {
        Task<List<ReviewDto>> GetReviews();
        Task<ReviewDto?> GetReview(Guid id);
        Task<bool> CreateReview(CreateReviewDto dto);
        Task<bool> UpdateReview(Guid id, UpdateReviewDto dto);
        Task<bool> DeleteReview(Guid id);
    }
}
