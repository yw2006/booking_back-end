using JwtAuthDotNet.Services.Interfaces;
using JwtAuthDotNet.Models.Hotel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JwtAuthDotNet.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class HotelsController(IHotelService hotelService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetHotels()
    {
        var hotels = await hotelService.GetHotels();
        return Ok(hotels);
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetHotel(int id)
    {
        HotelDto? hotel = await hotelService.GetHotel(id);
        if (hotel is null)
        {
            return NotFound();
        }

        return Ok(hotel);
    }

    [HttpPost("admin")]
    public async Task<IActionResult> AddHotel(CreateHotelDto dto, IFormFile? file)
    {
        bool wasSuccessful = await hotelService.CreateHotel(dto, file);
        if (!wasSuccessful)
        {
            return StatusCode(500, "Failed to create hotel");
        }

        return Ok();
    }

    [HttpPut("admin/{id:int}")]
    public async Task<IActionResult> UpdateHotel(int id, UpdateHotelDto dto, IFormFile? file)
    {
        bool wasSuccessful = await hotelService.UpdateHotel(id, dto, file);
        if (!wasSuccessful)
        {
            return NotFound("User with this Id was not found.");
        }

        return Ok();
    }

    [HttpDelete("admin/{id:int}")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        bool wasSuccessful = await hotelService.DeleteHotel(id);
        if (!wasSuccessful)
        {
            return NotFound("User with this Id was not found.");
        }

        return Ok();
    }
}
