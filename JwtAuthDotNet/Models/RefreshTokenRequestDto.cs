namespace JwtAuthDotNet.Models
{
    public class RefreshTokenRequestDto
    {
        public required string RefreshToken { get; set; } = string.Empty;
    }
}
