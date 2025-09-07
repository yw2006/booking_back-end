using JwtAuthDotNet.Entities;
using JwtAuthDotNet.Models;
using JwtAuthDotNet.Models.Hotel;

namespace JwtAuthDotNet.Services.Interfaces
{
    public interface IHotelService
    {
        Task<List<HotelDto>> GetHotels();
        Task<HotelDto?> GetHotel();
        Task<CreateHotelDto> CreateHotel();
        Task<UpdateHotelDto> UpdateHotelDto();
        Task<bool> DeleteHotel();

    }
}
