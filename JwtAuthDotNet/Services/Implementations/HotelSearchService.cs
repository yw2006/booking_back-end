using JwtAuthDotNet.Data;
using JwtAuthDotNet.Entities;
using JwtAuthDotNet.Enums;
using JwtAuthDotNet.Models.HotelSearch;
using JwtAuthDotNet.Services.Interfaces;
using JwtAuthDotNet.Validation;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthDotNet.Services.Implementations
{
    // Main search service
    public class HotelSearchService(UserDbContext context) : IHotelSearchService
    {



        public async Task<(bool Success, string Message, SearchResultsDto? Results)> SearchHotelsAsync(
            HotelSearchRequest request)
        {
            try
            {
                // Validate the search request
                var validation = ValidateSearchRequest(request);
                if (!validation.IsValid)
                {
                    return (false, string.Join("; ", validation.Errors), null);
                }

                // Normalize dates
                var checkIn = request.CheckIn.Date;
                var checkOut = request.CheckOut?.Date ?? checkIn.AddDays(1);
                var nights = (checkOut - checkIn).Days;

                // Build the query
                var hotelsQuery = context.Hotels
                    .Include(h => h.Rooms)
                        .ThenInclude(r => r.Bookings)
                    .Include(h => h.Reviews)
                    .Where(h => h.City.ToLower() == request.City.ToLower())
                    .AsQueryable();

                // Get hotels with available rooms
                var availableHotels = new List<HotelSearchResultDto>();

                var hotels = await hotelsQuery.ToListAsync();

                foreach (var hotel in hotels)
                {
                    var availableRooms = GetAvailableRooms(hotel, checkIn, checkOut, nights, request);

                    if (availableRooms.Any())
                    {
                        var hotelResult = new HotelSearchResultDto
                        {
                            Id = hotel.Id,
                            Name = hotel.Name,
                            City = hotel.City,
                            Address = hotel.Address,
                            Description = hotel.Description,
                            ThumbnailUrl = hotel.ThumbnailUrl,
                            AvailableRooms = availableRooms,
                            TotalAvailableRooms = availableRooms.Count,
                            MinPrice = availableRooms.Min(r => r.TotalPrice),
                            MaxPrice = availableRooms.Max(r => r.TotalPrice),
                            AverageRating = hotel.Reviews.Any() ? hotel.Reviews.Average(r => r.Rating) : null,
                            ReviewCount = hotel.Reviews.Count
                        };

                        availableHotels.Add(hotelResult);
                    }
                }

                // Apply sorting
                availableHotels = SortHotels(availableHotels, request);

                // Apply pagination
                var totalCount = availableHotels.Count;
                var totalPages = (int)Math.Ceiling((double)totalCount / request.PageSize);
                var skip = (request.Page - 1) * request.PageSize;

                var pagedHotels = availableHotels
                    .Skip(skip)
                    .Take(request.PageSize)
                    .ToList();

                var results = new SearchResultsDto
                {
                    Hotels = pagedHotels,
                    TotalCount = totalCount,
                    Page = request.Page,
                    PageSize = request.PageSize,
                    TotalPages = totalPages,
                    HasNextPage = request.Page < totalPages,
                    HasPreviousPage = request.Page > 1
                };

                return (true, $"Found {totalCount} available hotels", results);
            }
            catch (Exception ex)
            {
                return (false, "An error occurred while searching hotels", null);
            }
        }

        private List<AvailableRoomDto> GetAvailableRooms(
            Hotel hotel,
            DateTime checkIn,
            DateTime checkOut,
            int nights,
            HotelSearchRequest request)
        {
            var availableRooms = new List<AvailableRoomDto>();

            foreach (var room in hotel.Rooms.Where(r => r.IsAvailable))
            {
                // Check room capacity
                if (room.Capacity < request.Guests)
                    continue;

                // Check room type filter
                if (request.RoomTypes?.Any() == true && !request.RoomTypes.Contains(room.Name))
                    continue;

                // Check price range
                var totalPrice = room.BasePrice * nights;
                if (request.MinPrice.HasValue && totalPrice < request.MinPrice.Value)
                    continue;
                if (request.MaxPrice.HasValue && totalPrice > request.MaxPrice.Value)
                    continue;

                // Check availability (no conflicting bookings)
                var hasConflictingBooking = room.Bookings.Any(b =>
                    (b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed) &&
                    IsDateRangeOverlapping(checkIn, checkOut, b.CheckIn, b.CheckOut));

                if (!hasConflictingBooking)
                {
                    availableRooms.Add(new AvailableRoomDto
                    {
                        Id = room.Id,
                        HotelId = room.HotelId,
                        Name = room.Name,
                        Capacity = room.Capacity,
                        BasePrice = room.BasePrice,
                        TotalPrice = totalPrice,
                        Description = room.Description,
                        IsAvailable = room.IsAvailable,
                    });
                }
            }

            return availableRooms;
        }

        private static bool IsDateRangeOverlapping(DateTime start1, DateTime end1, DateTime start2, DateTime end2)
        {
            return start1 < end2 && end1 > start2;
        }

        private static List<HotelSearchResultDto> SortHotels(List<HotelSearchResultDto> hotels, HotelSearchRequest request)
        {
            // Default sort by price (lowest first)
            return hotels.OrderBy(h => h.MinPrice)
                        .ThenByDescending(h => h.AverageRating ?? 0)
                        .ThenBy(h => h.Name)
                        .ToList();
        }

        private static ValidationResult ValidateSearchRequest(HotelSearchRequest request)
        {
            var result = new ValidationResult { IsValid = true };

            if (string.IsNullOrWhiteSpace(request.City))
            {
                result.Errors.Add("City is required");
                result.IsValid = false;
            }

            var today = DateTime.Today;

            if (request.CheckIn.Date < today)
            {
                result.Errors.Add("Check-in date cannot be in the past");
                result.IsValid = false;
            }

            if (request.CheckOut.HasValue && request.CheckOut.Value.Date <= request.CheckIn.Date)
            {
                result.Errors.Add("Check-out date must be after check-in date");
                result.IsValid = false;
            }

            var maxDate = today.AddYears(2);
            if (request.CheckIn.Date > maxDate || (request.CheckOut.HasValue && request.CheckOut.Value.Date > maxDate))
            {
                result.Errors.Add("Search dates cannot be more than 2 years in advance");
                result.IsValid = false;
            }

            var maxStay = 365;
            if (request.CheckOut.HasValue)
            {
                var nights = (request.CheckOut.Value.Date - request.CheckIn.Date).Days;
                if (nights > maxStay)
                {
                    result.Errors.Add($"Maximum stay duration is {maxStay} nights");
                    result.IsValid = false;
                }
            }

            if (request.MinPrice.HasValue && request.MaxPrice.HasValue &&
                request.MinPrice.Value > request.MaxPrice.Value)
            {
                result.Errors.Add("Minimum price cannot be greater than maximum price");
                result.IsValid = false;
            }

            if (request.Page < 1)
            {
                result.Errors.Add("Page number must be greater than 0");
                result.IsValid = false;
            }

            if (request.PageSize < 1 || request.PageSize > 100)
            {
                result.Errors.Add("Page size must be between 1 and 100");
                result.IsValid = false;
            }

            return result;
        }

        // Quick availability check for a specific hotel
        public async Task<(bool Success, List<AvailableRoomDto> AvailableRooms)> CheckHotelAvailabilityAsync(
            Guid hotelId, DateTime checkIn, DateTime? checkOut, int guests = 1)
        {
            try
            {
                var checkInDate = checkIn.Date;
                var checkOutDate = checkOut?.Date ?? checkInDate.AddDays(1);
                var nights = (checkOutDate - checkInDate).Days;

                var hotel = await context.Hotels
                    .Include(h => h.Rooms)
                        .ThenInclude(r => r.Bookings)
                    .FirstOrDefaultAsync(h => h.Id == hotelId);

                if (hotel == null)
                    return (false, new List<AvailableRoomDto>());

                var request = new HotelSearchRequest
                {
                    City = hotel.City,
                    CheckIn = checkIn,
                    CheckOut = checkOut, // checkOut is now nullable, so this assignment is fine
                    Guests = guests
                };

                var availableRooms = GetAvailableRooms(hotel, checkInDate, checkOutDate, nights, request);

                return (true, availableRooms);
            }
            catch (Exception ex)
            {
                return (false, new List<AvailableRoomDto>());
            }
        }
    }
}
