using Microsoft.AspNetCore.Mvc;
using JwtAuthDotNet.Data;
using Microsoft.AspNetCore.Authorization;

namespace JwtAuthDotNet.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(UserDbContext context) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet("alladmins")]
    public IActionResult GetAllAdmins()
    {
        var users = context.Users.Where(u => u.Role.Equals("Admin"));

        return Ok(users);

    }

    [Authorize(Roles = "Admin")]
    [HttpGet("usercount")]
    public IActionResult GetUserCount()
    {
        int userCount = context.Users.Count(u => u.Role != "Admin");
        return Ok(new { count = userCount });
    }
}

