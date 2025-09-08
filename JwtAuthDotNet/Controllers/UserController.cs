using Microsoft.AspNetCore.Mvc;
using JwtAuthDotNet.Data;
using Microsoft.AspNetCore.Authorization;

namespace JwtAuthDotNet.Controllers;

[Route("api/[controller]")]
[ApiController]
public class UserController(UserDbContext context) : ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpGet("allusers")]
    public IActionResult GetAllUsers()
    {
        var users = context.Users;

        return Ok(users);

    }
}

