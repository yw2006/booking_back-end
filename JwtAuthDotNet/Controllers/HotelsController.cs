using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace JwtAuthDotNet.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class HotelsController() : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public IActionResult GetHotels()
    {
        return Ok();
    }

    [AllowAnonymous]
    [HttpGet]
    [Route("{id:int}")]
    public IActionResult GetHotel(int id)
    {
        return Ok();
    }

    [HttpPost]
    [Route("admin")]
    public IActionResult AddHotel()
    {
        return Ok();
    }

    [HttpPut]
    [Route("admin/{id:int}")]
    public IActionResult UpdateHotel(int id)
    {
        return Ok();
    }

    [HttpDelete]
    [Route("admin/{id:int}")]
    public IActionResult DeleteHotel(int id)
    {
        return Ok();
    }
}
