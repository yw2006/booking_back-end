using JwtAuthDotNet.Data;
using JwtAuthDotNet.Entities;
using JwtAuthDotNet.Models.Hotel;
using JwtAuthDotNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using FileTypeChecker;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthDotNet.Services.Implementations
{
    public class HotelService(UserDbContext context, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
    : IHotelService
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
        public async Task<HotelDto?> GetHotel(Guid id)
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
            };

            if (dto.Image is not null)
            {
                var request = httpContextAccessor.HttpContext?.Request;
                var baseUrl = $"{request?.Scheme}://{request?.Host}";

                hotel.ThumbnailUrl = await MakeImageURL(dto.Image, baseUrl);
            }

            context.Hotels.Add(hotel);
            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateHotel(Guid id, UpdateHotelDto dto)
        {
            Hotel? hotel = await context.Hotels.FindAsync(id);

            if (hotel is null)
            {
                return false;
            }

            hotel.Name = string.IsNullOrWhiteSpace(dto.Name) ? hotel.Name : dto.Name;
            hotel.City = string.IsNullOrWhiteSpace(dto.City) ? hotel.City : dto.City;
            hotel.Address = string.IsNullOrWhiteSpace(dto.Address) ? hotel.Address : dto.Address;
            hotel.Description = string.IsNullOrWhiteSpace(dto.Description) ? hotel.Description : dto.Description;

            if (dto.Image is not null)
            {
                var request = httpContextAccessor.HttpContext?.Request;
                var baseUrl = $"{request?.Scheme}://{request?.Host}";

                if (!string.IsNullOrEmpty(hotel.ThumbnailUrl))
                {
                    var oldFileName = Path.GetFileName(new Uri(hotel.ThumbnailUrl).LocalPath);
                    var oldFilePath = Path.Combine(env.WebRootPath, "images", oldFileName);
                    if (File.Exists(oldFilePath)) File.Delete(oldFilePath);
                }

                hotel.ThumbnailUrl = await MakeImageURL(dto.Image, baseUrl);
            }

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteHotel(Guid id)
        {
            Hotel? hotel = await context.Hotels.FindAsync(id);

            if (hotel is null)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(hotel.ThumbnailUrl))
            {
                var fileName = Path.GetFileName(new Uri(hotel.ThumbnailUrl).LocalPath);
                var filePath = Path.Combine(env.WebRootPath, "images", fileName);

                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
            }

            context.Hotels.Remove(hotel);

            await context.SaveChangesAsync();
            return true;
        }

        private async Task<string?> MakeImageURL(IFormFile file, string baseURL)
        {
            if (file == null)
            {
                return null;
            }

            using (var stream = file.OpenReadStream())
            {
                if (!FileTypeValidator.IsImage(stream))
                {
                    return null;
                }
            }

            string fileName;
            string filePath;

            do
            {
                fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                filePath = Path.Combine(env.WebRootPath, "images", fileName);

            } while (System.IO.File.Exists(filePath));

            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            string fileUrl = $"{baseURL}/images/{fileName}";

            return fileUrl;
        }
    }
}
