using JwtAuthDotNet.Data;
using JwtAuthDotNet.Entities;
using JwtAuthDotNet.Models.Hotel;
using JwtAuthDotNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthDotNet.Services.Implementations
{
    public class HotelService(UserDbContext context) : IHotelService
    {
        public async Task<List<HotelDto>> GetHotels()
        {
            List<HotelDto> hotels = await context.Hotels.Select(h => new HotelDto
            {
                Id = h.Id,
                Name = h.Name,
                City = h.City,
                Address = h.Address,
                Description = h.Description,
                ThumbnailUrl = h.ThumbnailUrl,
                CreatedAt = h.CreatedAt

            }).ToListAsync();


            return hotels;

        }
        public async Task<HotelDto?> GetHotel(int id)
        {
            Hotel? h = await context.Hotels.FindAsync(id);

            if (h is null)
            {
                return null;
            }

            return new HotelDto
            {
                Id = h.Id,
                Name = h.Name,
                City = h.City,
                Address = h.Address,
                Description = h.Description,
                ThumbnailUrl = h.ThumbnailUrl,
                CreatedAt = h.CreatedAt
            };
        }

        public async Task<bool> CreateHotel(CreateHotelDto dto)
        {
            Hotel hotel = new Hotel
            {
                Name = dto.Name,
                City = dto.City,
                Address = dto.Address,
                Description = dto.Description,
                ThumbnailUrl = dto.ThumbnailUrl,
            };

            context.Hotels.Add(hotel);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateHotel(int id, UpdateHotelDto dto)
        {
            Hotel? hotel = await context.Hotels.FindAsync(id);

            if (hotel is null)
            {
                return false;
            }

            hotel.Name = dto.Name;
            hotel.City = dto.City;
            hotel.Address = dto.Address;
            hotel.Description = dto.Description;
            hotel.ThumbnailUrl = dto.ThumbnailUrl;

            await context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteHotel(int id)
        {
            Hotel? hotel = await context.Hotels.FindAsync(id);

            if (hotel is null)
            {
                return false;
            }

            context.Hotels.Remove(hotel);

            await context.SaveChangesAsync();
            return true;
        }
    }
}
