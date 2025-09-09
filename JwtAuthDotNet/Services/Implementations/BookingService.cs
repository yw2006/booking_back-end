using JwtAuthDotNet.Data;
using JwtAuthDotNet.Entities;
using JwtAuthDotNet.Enums;
using JwtAuthDotNet.Models.Booking;
using JwtAuthDotNet.Models.Hotel;
using JwtAuthDotNet.Models.Room;
using JwtAuthDotNet.Services.Interfaces;
using JwtAuthDotNet.Validation;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthDotNet.Services.Implementations
{
    public class BookingService(UserDbContext context) : IBookingService
    {

        public async Task<(bool Success, string Message, Guid? BookingId)> CreateBookingAsync(
    CreateBookingDto request, Guid userId)
        {
            try
            {
                // Step 1: Validate the request
                var validation = await ValidateBookingRequestAsync(request, userId);
                if (!validation.IsValid)
                {
                    return (false, string.Join("; ", validation.Errors), null);
                }

                // Step 2: Normalize dates
                var checkIn = request.CheckIn.Date;
                var checkOut = checkIn.AddDays(request.Nights);

                // Step 3: Create the booking
                var booking = new Booking
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    RoomId = request.RoomId,
                    CheckIn = checkIn,
                    CheckOut = checkOut,
                    Nights = request.Nights,
                    Status = BookingStatus.Pending,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                context.Bookings.Add(booking);
                await context.SaveChangesAsync();

                return (true, "Booking created successfully", booking.Id);
            }
            catch (Exception ex)
            {
                return (false, "An error occurred while creating the booking", null);
            }
        }

        private async Task<BookingValidationResult> ValidateBookingRequestAsync(
            CreateBookingDto request, Guid userId)
        {
            var result = new BookingValidationResult { IsValid = true };

            // 1. Basic input validation
            if (request.RoomId == Guid.Empty)
            {
                result.Errors.Add("Invalid room ID");
                result.IsValid = false;
            }

            if (request.Nights <= 0)
            {
                result.Errors.Add("Number of nights must be greater than 0");
                result.IsValid = false;
            }

            if (request.Nights > 365)
            {
                result.Errors.Add("Maximum booking duration is 365 nights");
                result.IsValid = false;
            }

            // 2. Date validation
            var checkIn = request.CheckIn.Date;
            var today = DateTime.Today;

            if (checkIn < today)
            {
                result.Errors.Add("Check-in date cannot be in the past");
                result.IsValid = false;
            }

            if (checkIn > today.AddYears(2))
            {
                result.Errors.Add("Check-in date cannot be more than 2 years in advance");
                result.IsValid = false;
            }

            var checkOut = checkIn.AddDays(request.Nights);
            if (checkOut > today.AddYears(2))
            {
                result.Errors.Add("Check-out date cannot be more than 2 years in advance");
                result.IsValid = false;
            }

            // 3. User validation
            var userExists = await context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                result.Errors.Add("Invalid user");
                result.IsValid = false;
            }

            // 4. Room validation
            var room = await context.Rooms
                .Include(r => r.Bookings)
                .FirstOrDefaultAsync(r => r.Id == request.RoomId);

            if (room == null)
            {
                result.Errors.Add("Room not found");
                result.IsValid = false;
                return result; // Early return if room doesn't exist
            }

            // 5. Room availability check
            var conflictingBooking = room.Bookings.FirstOrDefault(b =>
                (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed) &&
                IsDateRangeOverlapping(checkIn, checkOut, b.CheckIn, b.CheckOut));

            if (conflictingBooking != null)
            {
                result.Errors.Add($"Room is not available for the selected dates. Conflicting booking from {conflictingBooking.CheckIn:yyyy-MM-dd} to {conflictingBooking.CheckOut:yyyy-MM-dd}");
                result.ConflictingBookingId = conflictingBooking.Id.ToString();
                result.IsValid = false;
            }

            // 6. Business rule validations

            // Check for duplicate pending bookings by same user for same room
            var hasPendingBooking = await context.Bookings.AnyAsync(b =>
                b.UserId == userId &&
                b.RoomId == request.RoomId &&
                b.Status == BookingStatus.Pending);

            if (hasPendingBooking)
            {
                result.Errors.Add("You already have a pending booking for this room");
                result.IsValid = false;
            }

            // Check maximum concurrent bookings per user
            var activeBookingsCount = await context.Bookings.CountAsync(b =>
                b.UserId == userId &&
                (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed) &&
                b.CheckOut > DateTime.Today);

            if (activeBookingsCount >= 5) // Configurable limit
            {
                result.Errors.Add("Maximum number of active bookings reached (5)");
                result.IsValid = false;
            }

            // Check minimum advance booking (e.g., must book at least 1 day in advance)
            if (checkIn <= today)
            {
                result.Errors.Add("Booking must be made at least 1 day in advance");
                result.IsValid = false;
            }

            return result;
        }

        private static bool IsDateRangeOverlapping(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            // Two date ranges overlap if: start1 < end2 AND end1 > start2
            return start1 < end2 && end1 > start2;
        }



        public async Task<IEnumerable<BookingDto>> GetBookingsByStatusAsync(string status)
        {
            if (!Enum.TryParse<BookingStatus>(status, true, out var parsedStatus))
                return Enumerable.Empty<BookingDto>();

            return await context.Bookings
                .Where(b => b.Status == parsedStatus).Include(b => b.Room)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    HotelId = b.Room.HotelId,
                    RoomId = b.Room.Id,
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut,
                    Nights = b.Nights,
                    TotalPrice = b.Room.BasePrice * b.Nights,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt,
                    Hotel = new HotelDto
                    {
                        Id = b.Room.Hotel.Id,
                        Name = b.Room.Hotel.Name,
                        Address = b.Room.Hotel.Address,
                        City = b.Room.Hotel.City,
                        ThumbnailUrl = b.Room.Hotel.ThumbnailUrl,
                        Description = b.Room.Hotel.Description,
                        CreatedAt = b.Room.Hotel.CreatedAt
                    },
                    Room = new RoomDto
                    {
                        Id = b.Room.Id,
                        HotelId = b.Room.HotelId,
                        Name = b.Room.Name,
                        Description = b.Room.Description,
                        BasePrice = b.Room.BasePrice,
                    }
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<BookingDto>> GetUserBookingsAsync(Guid userId)
        {
            return await context.Bookings.Where(b => b.UserId == userId).Include(b => b.Room)
                .Select(b => new BookingDto
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    HotelId = b.Room.HotelId,
                    RoomId = b.RoomId,
                    CheckIn = b.CheckIn,
                    CheckOut = b.CheckOut,
                    Nights = b.Nights,
                    TotalPrice = b.TotalPrice,
                    Status = b.Status,
                    CreatedAt = b.CreatedAt,
                    Hotel = new HotelDto
                    {
                        Id = b.Room.Hotel.Id,
                        Name = b.Room.Hotel.Name,
                        Address = b.Room.Hotel.Address,
                        City = b.Room.Hotel.City,
                        ThumbnailUrl = b.Room.Hotel.ThumbnailUrl,
                        Description = b.Room.Hotel.Description,
                        CreatedAt = b.Room.Hotel.CreatedAt
                    },
                    Room = new RoomDto
                    {
                        Id = b.Room.Id,
                        HotelId = b.Room.HotelId,
                        Name = b.Room.Name,
                        Description = b.Room.Description,
                        BasePrice = b.Room.BasePrice,
                    }
                }).ToListAsync();
        }

        public async Task<bool> UpdateBookingStatusAsync(Guid bookingId, BookingStatus status)
        {
            var booking = await context.Bookings.FirstOrDefaultAsync(b => b.Id == bookingId);
            if (booking == null) return false;


            switch (booking.Status)
            {
                case BookingStatus.Pending:
                    if (status == BookingStatus.Confirmed ||
                        status == BookingStatus.Rejected ||
                        status == BookingStatus.Cancelled)
                    {
                        booking.Status = status;
                    }
                    else
                    {
                        return false;
                    }
                    break;

                case BookingStatus.Confirmed:
                    if (status == BookingStatus.Completed ||
                        status == BookingStatus.Cancelled)
                    {
                        booking.Status = status;
                    }
                    else
                    {
                        return false;
                    }
                    break;

                default:
                    return false; // cannot update status if already Rejected, Cancelled, or Completed
            }

            booking.UpdatedAt = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return true;
        }

    }
}
