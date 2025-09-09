namespace JwtAuthDotNet.Validation
{    public class BookingValidationResult: ValidationResult
    {

        public string? ConflictingBookingId { get; set; }
    }
}
