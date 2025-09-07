using JwtAuthDotNet.Data;
using JwtAuthDotNet.Entities;
using JwtAuthDotNet.Models.Hotel;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace JwtAuthDotNet.Services.Implementations
{
    public class HotelService(UserDbContext context)
    {
        public async Task<List<HotelDto>> GetHotels()
        {
            return [];

        }
        public async Task<HotelDto?> GetHotel()
        {
            return null;
        }
        public async Task<CreateHotelDto> CreateHotel()
        {
            return new CreateHotelDto();
        }
        public async Task<UpdateHotelDto> UpdateHotelDto()
        {

            return new UpdateHotelDto();

        }
        public async Task<bool> DeleteHotel()
        {

            return false;
        }
    }
}
