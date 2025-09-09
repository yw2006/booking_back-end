using JwtAuthDotNet.Services.Interfaces;
using JwtAuthDotNet.Models.Room;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JwtAuthDotNet.Controllers;

[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class RoomsController(IRoomService roomService) : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetRooms()
    {
        var rooms = await roomService.GetRooms();
        return Ok(rooms);
    }

    [AllowAnonymous]
    [HttpGet("{id:Guid}")]
    public async Task<IActionResult> GetRoom(Guid id)
    {
        RoomDto? room = await roomService.GetRoom(id);
        if (room is null)
        {
            return NotFound();
        }

        return Ok(room);
    }

    [HttpPost("admin")]
    public async Task<IActionResult> AddRoom([FromBody] CreateRoomDto dto)
    {
        bool wasSuccessful = await roomService.CreateRoom(dto);

        if (!wasSuccessful)
        {
            return StatusCode(500, new { message = "Failed to create hotel" });
        }

        return Ok();
    }

    [HttpPut("admin/{id:Guid}")]
    public async Task<IActionResult> UpdateRoom(Guid id, [FromBody] UpdateRoomDto dto)
    {
        bool wasSuccessful = await roomService.UpdateRoom(id, dto);
        if (!wasSuccessful)
        {
            return NotFound(new { message = "Room with this Id was not found." });
        }

        return Ok();
    }

    [HttpDelete("admin/{id:Guid}")]
    public async Task<IActionResult> DeleteRoom(Guid id)
    {
        bool wasSuccessful = await roomService.DeleteRoom(id);
        if (!wasSuccessful)
        {
            return NotFound(new { message = "Room with this Id was not found." });
        }

        return Ok();
    }
}
