using JwtAuthDotNet.Entities;
using JwtAuthDotNet.Models;
using JwtAuthDotNet.Models.Hotel;

namespace JwtAuthDotNet.Services.Interfaces
{
    public interface IHotelService
    {
        Task<List<HotelDto>> GetHotels();
        Task<HotelDto?> GetHotel(int id);
        Task<bool> CreateHotel(CreateHotelDto dto, IFormFile? file);
        Task<bool> UpdateHotel(int id, UpdateHotelDto dto, IFormFile? file);
        Task<bool> DeleteHotel(int id);

    }
}
