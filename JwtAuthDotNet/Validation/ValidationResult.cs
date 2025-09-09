﻿namespace JwtAuthDotNet.Validation
{
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public List<string> Errors { get; set; } = new();
    }
}
