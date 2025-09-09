
using JwtAuthDotNet.Data;
using JwtAuthDotNet.Entities;
using JwtAuthDotNet.Models.Review;
using JwtAuthDotNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthDotNet.Services.Implementations
{
    public class ReviewService(UserDbContext context) : IReviewsService
    {
        public async Task<List<ReviewDto>> GetHotelReviews(Guid hotelId)
        {
            return await context.Reviews
                .Where(r => r.HotelId == hotelId)
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

        public async Task<ReviewDto?> GetReview(Guid reviewid)
        {
            var review = await context.Reviews.FindAsync(reviewid);
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

        public async Task<(bool Success, string Message, Guid? ReviewId)> CreateReview(CreateReviewDto dto, Guid userId, Guid hotelId)
        {
            if (dto.Rating < 1 || dto.Rating > 5)
            {
                return (false, "Rating must be between 1 and 5.", null);
            }

            var hasBooked = await context.Bookings.AnyAsync(b => b.UserId == userId && b.Room.HotelId == hotelId && b.Status == Enums.BookingStatus.Completed);

            if (!hasBooked)
            {
                return (false, "You have not booked this hotel before or your booking is not completed.", null);
            }

            var review = new Review
            {
                UserId = userId,
                HotelId = hotelId,
                Rating = dto.Rating,
                Comment = dto.Comment,
            };

            context.Reviews.Add(review);
            await context.SaveChangesAsync();
            return (true, "Review created successfully.", review.Id);
        }

        public async Task<(bool Success, string Message)> UpdateReview(Guid reviewid, UpdateReviewDto dto, Guid userId)
        {
            var review = await context.Reviews.FindAsync(reviewid);
            if (review is null) return (false, "Review not found.");

            if (review.UserId != userId)
            {
                return (false, "You are not authorized to update this review.");
            }

            if (dto.Rating.HasValue)
            {
                if (dto.Rating.Value < 1 || dto.Rating.Value > 5)
                {
                    return (false, "Rating must be between 1 and 5.");
                }
                review.Rating = dto.Rating.Value;
            }

            if (string.IsNullOrWhiteSpace(dto.Comment))
                review.Comment = dto.Comment;

            review.UpdatedAt = System.DateTime.UtcNow;

            await context.SaveChangesAsync();
            return (true, "Review updated successfully.");
        }

        public async Task<(bool Success, string Message)> DeleteReview(Guid reviewid, Guid userId)
        {
            var review = await context.Reviews.FindAsync(reviewid);
            if (review is null) return (false, "Review not found.");

            if (review.UserId != userId)
            {
                return (false, "You are not authorized to delete this review.");
            }

            context.Reviews.Remove(review);
            await context.SaveChangesAsync();
            return (true, "Review deleted successfully.");
        }
    }
}
