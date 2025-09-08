using JwtAuthDotNet.Services.Interfaces;
using JwtAuthDotNet.Models.Review;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JwtAuthDotNet.Controllers;

[AllowAnonymous]
[Route("api/[controller]")]
[ApiController]
public class ReviewsController(IReviewsService reviewsService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetReviews()
    {
        var reviews = await reviewsService.GetReviews();
        return Ok(reviews);
    }

    [AllowAnonymous]
    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetReview(Guid id)
    {
        var review = await reviewsService.GetReview(id);
        if (review is null) return NotFound();
        return Ok(review);
    }

    [HttpPost]
    public async Task<IActionResult> AddReview([FromBody] CreateReviewDto dto)
    {
        bool wasSuccessful = await reviewsService.CreateReview(dto);
        if (!wasSuccessful) return StatusCode(500, "Failed to create review");
        return Ok();
    }

    [HttpPut("{id:Guid}")]
    public async Task<IActionResult> UpdateReview(Guid id, [FromBody] UpdateReviewDto dto)
    {
        bool wasSuccessful = await reviewsService.UpdateReview(id, dto);
        if (!wasSuccessful) return NotFound("Review with this Id was not found.");
        return Ok();
    }

    [HttpDelete("{id:Guid}")]
    public async Task<IActionResult> DeleteReview(Guid id)
    {
        bool wasSuccessful = await reviewsService.DeleteReview(id);
        if (!wasSuccessful) return NotFound("Review with this Id was not found.");
        return Ok();
    }
}
