using JwtAuthDotNet.Services.Interfaces;
using JwtAuthDotNet.Models.Hotel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JwtAuthDotNet.Controllers;

[Authorize(Roles = "Admin")]
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
    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetHotel(Guid id)
    {
        HotelDto? hotel = await hotelService.GetHotel(id);
        if (hotel is null)
        {
            return NotFound();
        }

        return Ok(hotel);
    }

    [HttpPost("admin")]
    public async Task<IActionResult> AddHotel([FromForm] CreateHotelDto dto)
    {
        bool wasSuccessful = await hotelService.CreateHotel(dto);

        if (!wasSuccessful)
        {
            return StatusCode(500, new { message = "Failed to create hotel" });
        }

        if (dto.Image == null || dto.Image.Length == 0)
        {
            return BadRequest();
        }

        return Ok();
    }

    [HttpPut("admin/{id:Guid}")]
    public async Task<IActionResult> UpdateHotel(Guid id, UpdateHotelDto dto)
    {
        bool wasSuccessful = await hotelService.UpdateHotel(id, dto);
        if (!wasSuccessful)
        {
            return NotFound(new { message = "User with this Id was not found." });
        }

        return Ok();
    }

    [HttpDelete("admin/{id:Guid}")]
    public async Task<IActionResult> DeleteHotel(Guid id)
    {
        bool wasSuccessful = await hotelService.DeleteHotel(id);
        if (!wasSuccessful)
        {
            return NotFound(new { message = "User with this Id was not found." });
        }

        return Ok();
    }
}
