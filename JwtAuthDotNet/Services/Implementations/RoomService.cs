using JwtAuthDotNet.Data;
using JwtAuthDotNet.Entities;
using JwtAuthDotNet.Models.Room;
using JwtAuthDotNet.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthDotNet.Services.Implementations
{
    public class RoomService(UserDbContext context) : IRoomService
    {
        public async Task<List<RoomDto>> GetRooms()
        {
            var rooms = await context.Rooms
                .Select(r => new RoomDto
                {
                    Id = r.Id,
                    HotelId = r.HotelId,
                    Name = r.Name,
                    BasePrice = r.BasePrice,
                    Description = r.Description,
                    IsAvailable = r.IsAvailable,
                    CreatedAt = r.CreatedAt,
                    UpdatedAt = r.UpdatedAt,
                })
                .ToListAsync();

            return rooms;
        }

        public async Task<RoomDto?> GetRoom(Guid id)
        {
            var room = await context.Rooms
                .Include(r => r.Bookings)
                .FirstOrDefaultAsync(r => r.Id == id);

            if (room is null)
            {
                return null;
            }

            return new RoomDto
            {
                Id = room.Id,
                HotelId = room.HotelId,
                Name = room.Name,
                BasePrice = room.BasePrice,
                Description = room.Description,
                IsAvailable = room.IsAvailable,
                CreatedAt = room.CreatedAt,
                UpdatedAt = room.UpdatedAt,
            };
        }

        public async Task<bool> CreateRoom(CreateRoomDto dto)
        {
            var room = new Room
            {
                HotelId = dto.HotelId,
                Name = dto.Name,
                BasePrice = dto.BasePrice,
                Description = dto.Description,
                IsAvailable = dto.IsAvailable,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
            };

            context.Rooms.Add(room);
            await context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UpdateRoom(Guid id, UpdateRoomDto dto)
        {
            var room = await context.Rooms.FindAsync(id);

            if (room is null)
            {
                return false;
            }

            if (dto.Name.HasValue)
                room.Name = dto.Name.Value;

            if (dto.BasePrice.HasValue)
                room.BasePrice = dto.BasePrice.Value;

            if (dto.Description is not null)
                room.Description = dto.Description;

            if (dto.IsAvailable.HasValue)
                room.IsAvailable = dto.IsAvailable.Value;

            room.UpdatedAt = DateTime.UtcNow;

            await context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRoom(Guid id)
        {
            var room = await context.Rooms.FindAsync(id);

            if (room is null)
            {
                return false;
            }

            context.Rooms.Remove(room);
            await context.SaveChangesAsync();

            return true;
        }
    }
}
