using JwtAuthDotNet.Entities;
using JwtAuthDotNet.Models;
using JwtAuthDotNet.Models.Hotel;

namespace JwtAuthDotNet.Services.Interfaces
{
    public interface IHotelService
    {
        Task<List<HotelDto>> GetHotels();
        Task<HotelDto?> GetHotel(Guid id);
        Task<bool> CreateHotel(CreateHotelDto dto);
        Task<bool> UpdateHotel(Guid id, UpdateHotelDto dto);
        Task<bool> DeleteHotel(Guid id);

    }
}
