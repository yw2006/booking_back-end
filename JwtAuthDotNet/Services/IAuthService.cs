using JwtAuthDotNet.Entities;
using JwtAuthDotNet.Models;

namespace JwtAuthDotNet.Services
{
    public interface IAuthService
    {
        Task<TokenResponseDto?> RegisterAsync(UserDto request);
        Task<TokenResponseDto?> LoginAsync(UserDto request);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}
