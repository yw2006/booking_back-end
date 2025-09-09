using JwtAuthDotNet.Enums;
using JwtAuthDotNet.Models.Booking;

namespace JwtAuthDotNet.Services.Interfaces
{
    public interface IBookingService
    {
        Task<(bool Success, string Message, Guid? BookingId)> CreateBookingAsync(CreateBookingDto model,Guid userId);
        Task<IEnumerable<BookingDto>> GetUserBookingsAsync(Guid userId);
        Task<IEnumerable<BookingDto>> GetBookingsByStatusAsync(string status);
        Task<bool> UpdateBookingStatusAsync(Guid bookingId, BookingStatus status);

    }
}
