﻿namespace JwtAuthDotNet.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string HashPassword { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
     
        }
}
