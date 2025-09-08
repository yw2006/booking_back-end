using JwtAuthDotNet.Models;
using JwtAuthDotNet.Models.User;

namespace JwtAuthDotNet.Services.Interfaces
{
    public interface IAuthService
    {
        Task<TokenResponseDto?> RegisterAsync(UserDto request);
        Task<TokenResponseDto?> LoginAsync(LoginUserDto request);
        Task<bool> CreateAdmin(UserDto request);
        Task<TokenResponseDto?> RefreshTokenAsync(RefreshTokenRequestDto request);
    }
}
