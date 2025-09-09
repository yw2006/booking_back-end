using JwtAuthDotNet.Enums;
using JwtAuthDotNet.Models.Booking;
using JwtAuthDotNet.Services.Implementations;
using JwtAuthDotNet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JwtAuthDotNet.Controllers
{
    [Route("api")]
    [ApiController]
    public class BookingController(IBookingService bookingService) : ControllerBase
    {
        [HttpPost("booking")]
        public async Task<IActionResult> CreateBooking([FromBody]  CreateBookingDto request)
        {
            // Get user ID from JWT token
            var userId = GetUserIdFromToken();

            if (userId == Guid.Empty)
                return Unauthorized();

            var (success, message, bookingId) = await bookingService.CreateBookingAsync(request, userId);

            if (success)
            {
                return Ok(new
                {
                    Success = true,
                    Message = message,
                    BookingId = bookingId
                });
            }

            return BadRequest(new
            {
                Success = false,
                Message = message
            });
        }
        [Authorize]
        [HttpGet("me/bookings")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetMyBookings()
        {
            var userIdClaim = HttpContext.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return Unauthorized("User ID not found in token.");
            }

            var userId = Guid.Parse(userIdClaim);

            var bookings = await bookingService.GetUserBookingsAsync(userId);

            return Ok(bookings);
        }


        [Authorize]
        [HttpPatch("bookings/{id}/cancel")]
        public async Task<ActionResult> CancelBooking(Guid id)
        {
            var result = await bookingService.UpdateBookingStatusAsync(id,BookingStatus.Cancelled);

            if (!result)
                return BadRequest("Failed to cancel booking.");

            return Ok("Booking cancelled successfully.");
        }

       
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/bookings")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetPendingBookings([FromQuery] string status = "PENDING")
        {
            var bookings = await bookingService.GetBookingsByStatusAsync(status);
            return Ok(bookings);
        }

  
        [Authorize(Roles = "Admin")]
        [HttpPatch("admin/bookings/{id}/confirm")]
        public async Task<ActionResult> ConfirmBooking(Guid id)
        {
            var result = await bookingService.UpdateBookingStatusAsync(id, BookingStatus.Confirmed);

            if (!result)
                return BadRequest("Failed to confirm booking.");

            return Ok("Booking confirmed successfully.");
        }

        
        [Authorize(Roles = "Admin")]
        [HttpPatch("admin/bookings/{id}/reject")]
        public async Task<ActionResult> RejectBooking(Guid id)
        {
            var result = await bookingService.UpdateBookingStatusAsync(id, BookingStatus.Rejected);

            if (!result)
                return BadRequest("Failed to reject booking.");

            return Ok("Booking rejected successfully.");
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("admin/bookings/{id}/complete")]
        public async Task<ActionResult> CompleteBooking(Guid id)
        {
            var result = await bookingService.UpdateBookingStatusAsync(id, BookingStatus.Completed);

            if (!result)
                return BadRequest("Failed to complete booking.");

            return Ok("Booking completed successfully.");
        }
        private Guid GetUserIdFromToken()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
        }
    }

}
