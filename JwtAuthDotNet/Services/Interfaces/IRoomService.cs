using JwtAuthDotNet.Entities;
using JwtAuthDotNet.Models.Room;

namespace JwtAuthDotNet.Services.Interfaces
{
    public interface IRoomService
    {
        Task<List<RoomDto>> GetRooms();
        Task<RoomDto?> GetRoom(Guid id);
        Task<bool> CreateRoom(CreateRoomDto dto);
        Task<bool> UpdateRoom(Guid id, UpdateRoomDto dto);
        Task<bool> DeleteRoom(Guid id);

    }
}
