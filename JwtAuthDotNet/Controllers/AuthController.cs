using JwtAuthDotNet.Entities;
using JwtAuthDotNet.Models;
using JwtAuthDotNet.Models.User;
using JwtAuthDotNet.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthDotNet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : ControllerBase
    {
        public User user = new();

        [HttpPost("register")]
        public async Task<ActionResult<TokenResponseDto>> Register(UserDto request)
        {
            var result = await authService.RegisterAsync(request);
            if (result is null)
            {
                return BadRequest(new { message="User already exists"});
            }
            return Ok(new { result });
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenResponseDto>> Login(LoginUserDto request)
        {
            var result = await authService.LoginAsync(request);
            if (result is null)
            {
                return BadRequest("Wrong username or password");
            }
            return Ok(result);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<TokenResponseDto>> RefreshToken(RefreshTokenRequestDto request)
        {
            var result = await authService.RefreshTokenAsync(request);
            if (result is null || result.RefreshToken is null || result.AccessToken is null)
            {
                return BadRequest("Invalid client request");
            }
            return Ok(result);
        }

        [Authorize]
        [HttpGet]
        public IActionResult AuthenticatedOnlyEndPoints()
        {
            return Ok($"okay your authenticated 👍");
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnlyEndPoints()
        {
            return Ok($"okay your Admin 👍");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("admin/create-admin")]
        public async Task<IActionResult> CreateAdmin(UserDto request)
        {
            bool result = await authService.CreateAdmin(request);
            if (!result)
            {
                return BadRequest("User already exists");
            }
            return Ok(result);
        }
    }
}
