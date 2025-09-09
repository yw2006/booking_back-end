using JwtAuthDotNet.Services.Interfaces;
using JwtAuthDotNet.Models.Review;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using JwtAuthDotNet.utils;

namespace JwtAuthDotNet.Controllers;

[Route("api/reviews")]
[ApiController]
public class ReviewsController(IReviewsService reviewsService) : ControllerBase
{

    [AllowAnonymous]
    [HttpGet("hotel/{hotelId:Guid}")]
    public async Task<IActionResult> GetHotelReviews(Guid hotelId)
    {
        var reviews = await reviewsService.GetHotelReviews(hotelId);
        return Ok(reviews);
    }

    [AllowAnonymous]
    [HttpGet("{reviewId:Guid}")]
    public async Task<IActionResult> GetReview(Guid reviewId)
    {
        var review = await reviewsService.GetReview(reviewId);
        if (review is null) return NotFound();
        return Ok(review);
    }

    [Authorize]
    [HttpPost("hotel/{hotelId:Guid}")]
    public async Task<IActionResult> AddReview(Guid hotelId, [FromBody] CreateReviewDto dto)
    {
        var userId = GetUserIdFromToken.ExtractUserId(HttpContext);
        if (userId == Guid.Empty)
        {
            return Unauthorized();
        }

        var (success, message, reviewId) = await reviewsService.CreateReview(dto, userId, hotelId);
        if (!success)
        {
            return BadRequest(message);
        }

        return Ok(message);
    }

    [Authorize]
    [HttpPut("{reviewId:Guid}")]
    public async Task<IActionResult> UpdateReview(Guid reviewId, [FromBody] UpdateReviewDto dto)
    {
        var userId = GetUserIdFromToken.ExtractUserId(HttpContext);

        if (userId == Guid.Empty)
        {
            return Unauthorized();
        }

        var (success, message) = await reviewsService.UpdateReview(reviewId, dto, userId);

        if (!success)
        {
            return BadRequest(message);
        }

        return Ok(message);
    }

    [Authorize]
    [HttpDelete("{reviewId:Guid}")]
    public async Task<IActionResult> DeleteReview(Guid reviewId)
    {
        var userId = GetUserIdFromToken.ExtractUserId(HttpContext);

        if (userId == Guid.Empty)
        {
            return Unauthorized();
        }

        var (success, message) = await reviewsService.DeleteReview(reviewId, userId);
        if (!success)
        {
            return BadRequest(message);
        }

        return Ok(message);
    }
}
