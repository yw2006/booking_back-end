using JwtAuthDotNet.Models.HotelSearch;

namespace JwtAuthDotNet.Services.Interfaces
{
    public interface IHotelSearchService
    {
        public Task<(bool Success, string Message, SearchResultsDto? Results)> SearchHotelsAsync(
            HotelSearchRequest request);
        public Task<(bool Success, List<AvailableRoomDto> AvailableRooms)> CheckHotelAvailabilityAsync(
            Guid hotelId, DateTime checkIn, DateTime? checkOut, int guests);

    }
}
