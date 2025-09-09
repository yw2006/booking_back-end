using JwtAuthDotNet.Enums;
using JwtAuthDotNet.Models.Booking;
using JwtAuthDotNet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using JwtAuthDotNet.utils;
namespace JwtAuthDotNet.Controllers
{
    [Route("api")]
    [ApiController]
    public class BookingController(IBookingService bookingService) : ControllerBase
    {
        [HttpPost("booking")]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto request)
        {
            // Get user ID from JWT token
            var userId = GetUserIdFromToken.ExtractUserId(HttpContext);

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
                return Unauthorized(new { message = "User ID not found in token." });
            }

            var userId = Guid.Parse(userIdClaim);

            var bookings = await bookingService.GetUserBookingsAsync(userId);

            return Ok(bookings);
        }

        [Authorize]
        [HttpPatch("bookings/{bookingid}/cancel")]
        public async Task<ActionResult> CancelBooking(Guid bookingid)
        {
            var result = await bookingService.UpdateBookingStatusAsync(bookingid, BookingStatus.Cancelled);

            if (!result)
                return BadRequest(new { message = "Failed to cancel booking." });

            return Ok(new { message = "Booking cancelled successfully." });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/bookings")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetPendingBookings([FromQuery] string status = "PENDING")
        {
            var bookings = await bookingService.GetBookingsByStatusAsync(status);
            return Ok(bookings);
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("admin/bookings/{bookingid}/confirm")]
        public async Task<ActionResult> ConfirmBooking(Guid bookingid)
        {
            var result = await bookingService.UpdateBookingStatusAsync(bookingid, BookingStatus.Confirmed);

            if (!result)
                return BadRequest(new { message = "Failed to confirm booking." });

            return Ok(new { message = "Booking confirmed successfully." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("admin/bookings/{bookingid}/reject")]
        public async Task<ActionResult> RejectBooking(Guid bookingid)
        {
            var result = await bookingService.UpdateBookingStatusAsync(bookingid, BookingStatus.Rejected);

            if (!result)
                return BadRequest("Failed to reject booking.");

            return Ok(new { message = "Booking rejected successfully." });
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("admin/bookings/{bookingid}/complete")]
        public async Task<ActionResult> CompleteBooking(Guid bookingid)
        {
            var result = await bookingService.UpdateBookingStatusAsync(bookingid, BookingStatus.Completed);

            if (!result)
                return BadRequest(new { message = "Failed to complete booking." });

            return Ok(new { message = "Booking completed successfully." });
        }
    }
}
