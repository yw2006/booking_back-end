using JwtAuthDotNet.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JwtAuthDotNet.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class HotelsController(IHotelService IHotelService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetHotels()
    {
        await Task.CompletedTask; // replace with: await _hotelService.GetHotelsAsync();
        return Ok();
    }

    [AllowAnonymous]
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetHotel(int id)
    {
        await Task.CompletedTask; // replace with: await _hotelService.GetHotelByIdAsync(id);
        return Ok();
    }

    [HttpPost("admin")]
    public async Task<IActionResult> AddHotel()
    {
        await Task.CompletedTask; // replace with: await _hotelService.AddHotelAsync(model);
        return Ok();
    }

    [HttpPut("admin/{id:int}")]
    public async Task<IActionResult> UpdateHotel(int id)
    {
        await Task.CompletedTask; // replace with: await _hotelService.UpdateHotelAsync(id, model);
        return Ok();
    }

    [HttpDelete("admin/{id:int}")]
    public async Task<IActionResult> DeleteHotel(int id)
    {
        await Task.CompletedTask; // replace with: await _hotelService.DeleteHotelAsync(id);
        return Ok();
    }
}
