using JwtAuthDotNet.Models.HotelSearch;
using JwtAuthDotNet.Services.Implementations;
using JwtAuthDotNet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [AllowAnonymous]
    public class HotelSearchController(IHotelSearchService searchService) : ControllerBase
    {
        [HttpGet("search")]
        public async Task<IActionResult> SearchHotels([FromQuery] HotelSearchRequest request)
        {
            var (success, message, results) = await searchService.SearchHotelsAsync(request);

            if (success)
            {
                return Ok(new
                {
                    Success = true,
                    Message = message,
                    Data = results
                });
            }

            return BadRequest(new
            {
                Success = false,
                Message = message
            });
        }

        [HttpGet("hotels/{hotelId}/availability")]
        public async Task<IActionResult> CheckHotelAvailability(
            Guid hotelId,
            [FromQuery] DateTime checkIn,
            [FromQuery] DateTime? checkOut,
            [FromQuery] int guests = 1)
        {
            var (success, availableRooms) = await searchService
                .CheckHotelAvailabilityAsync(hotelId, checkIn, checkOut, guests);

            if (success)
            {
                return Ok(new
                {
                    Success = true,
                    HotelId = hotelId,
                    AvailableRooms = availableRooms,
                    TotalAvailableRooms = availableRooms.Count
                });
            }

            return BadRequest(new
            {
                Success = false,
                Message = "Hotel not found or error checking availability"
            });
        }
    }
}
