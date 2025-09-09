namespace JwtAuthDotNet.Models
{
    public class TokenResponseDto
    {
        public required string Name { get; set; } = string.Empty;
        public required string AccessToken { get; set; } = string.Empty;
        public required string RefreshToken { get; set; } = string.Empty;
        public required string Role { get; set; } = string.Empty;
    }
}
