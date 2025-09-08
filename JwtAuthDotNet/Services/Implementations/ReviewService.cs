using JwtAuthDotNet.Data;
using JwtAuthDotNet.Entities;
using JwtAuthDotNet.Models.Review;
using JwtAuthDotNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthDotNet.Services.Implementations
{
    public class ReviewService(UserDbContext context) : IReviewsService
    {
        public async Task<List<ReviewDto>> GetReviews()
        {
            return await context.Reviews
                .Select(r => new ReviewDto
                {
                    Id = r.Id,
                    UserId = r.UserId,
                    HotelId = r.HotelId,
                    Rating = r.Rating,
                    Comment = r.Comment,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt
                })
                .ToListAsync();
        }

        public async Task<ReviewDto?> GetReview(Guid id)
        {
            var review = await context.Reviews.FindAsync(id);
            if (review is null) return null;

            return new ReviewDto
            {
                Id = review.Id,
                UserId = review.UserId,
                HotelId = review.HotelId,
                Rating = review.Rating,
                Comment = review.Comment,
                CreatedAt = review.CreatedAt,
                UpdatedAt = review.UpdatedAt
            };
        }

        public async Task<bool> CreateReview(CreateReviewDto dto)
        {
            var review = new Review
            {
                UserId = dto.UserId,
                HotelId = dto.HotelId,
                Rating = dto.Rating,
                Comment = dto.Comment,
            };

            context.Reviews.Add(review);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateReview(Guid id, UpdateReviewDto dto)
        {
            var review = await context.Reviews.FindAsync(id);
            if (review is null) return false;

            if (dto.UserId.HasValue)
                review.UserId = dto.UserId.Value;

            if (dto.HotelId.HasValue)
                review.HotelId = dto.HotelId.Value;

            if (dto.Rating.HasValue)
                review.Rating = dto.Rating.Value;

            if (dto.Comment != null)
                review.Comment = dto.Comment;

            if (dto.CreatedAt.HasValue)
                review.CreatedAt = dto.CreatedAt.Value;

            review.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteReview(Guid id)
        {
            var review = await context.Reviews.FindAsync(id);
            if (review is null) return false;

            context.Reviews.Remove(review);
            await context.SaveChangesAsync();
            return true;
        }
    }
}
