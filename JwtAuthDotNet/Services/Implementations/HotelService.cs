using JwtAuthDotNet.Data;
using JwtAuthDotNet.Entities;
using JwtAuthDotNet.Models.Hotel;
using JwtAuthDotNet.Models.Room;
using JwtAuthDotNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using FileTypeChecker;

namespace JwtAuthDotNet.Services.Implementations
{
    public class HotelService(UserDbContext context, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
    : IHotelService
    {
        public async Task<List<HotelDto>> GetHotels()
        {
            List<HotelDto> hotels = await context.Hotels
                    .Select(h => new HotelDto
                    {
                        Id = h.Id,
                        Name = h.Name,
                        City = h.City,
                        Address = h.Address,
                        Description = h.Description,
                        ThumbnailUrl = h.ThumbnailUrl,
                        CreatedAt = h.CreatedAt,
                        Rooms = h.Rooms.Select(r => new RoomDto
                        {
                            Id = r.Id,
                            HotelId = r.HotelId,
                            Name = r.Name,
                            BasePrice = r.BasePrice,
                            Description = r.Description,
                            IsAvailable = r.IsAvailable,
                            CreatedAt = r.CreatedAt,
                            UpdatedAt = r.UpdatedAt

                        }).ToList()
                    }).ToListAsync();

            return hotels;

        }

        public async Task<HotelDto?> GetHotel(Guid id)
        {
            Hotel? h = await context.Hotels
                    .Include(h => h.Rooms)
                    .Include(h => h.Bookings)
                    .Include(h => h.Reviews)
                    .FirstOrDefaultAsync(h => h.Id == id);
            List<RoomDto> rooms = new List<RoomDto>();

            foreach (Room? room in h.Rooms)
            {
                rooms.Add(new RoomDto
                {
                    Id = room.Id,
                    HotelId = room.HotelId,
                    Name = room.Name,
                    BasePrice = room.BasePrice,
                    Description = room.Description,
                    IsAvailable = room.IsAvailable,
                    CreatedAt = room.CreatedAt,
                    UpdatedAt = room.UpdatedAt

                });
            }

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
                CreatedAt = h.CreatedAt,
                Rooms = rooms

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
            if (dto.City != null) hotel.City = dto.City;
            if (dto.Address != null) hotel.Address = dto.Address;
            if (dto.Description != null) hotel.Description = dto.Description;

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
