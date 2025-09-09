using JwtAuthDotNet.Enums;
using JwtAuthDotNet.Models.Hotel;
using JwtAuthDotNet.Models.Room;
using JwtAuthDotNet.Services.Interfaces;

namespace JwtAuthDotNet.testing
{
    public static class DbSeeder
    {
        public static async Task SeedDatabase(IHotelService hotelService, IRoomService roomService)
        {
            // Seed Hotels
            var hotels = await hotelService.GetHotels();
            if (hotels.Count == 0)
            {
                var hotel1 = new CreateHotelDto
                {
                    Name = "Grand Hyatt",
                    City = "New York",
                    Address = "109 East 42nd Street",
                    Description = "A luxury hotel in the heart of Manhattan."
                };

                var hotel2 = new CreateHotelDto
                {
                    Name = "The Ritz-Carlton",
                    City = "San Francisco",
                    Address = "600 Stockton St",
                    Description = "A timeless luxury hotel in Nob Hill."
                };

                await hotelService.CreateHotel(hotel1);
                await hotelService.CreateHotel(hotel2);

                // Get the created hotels to get their IDs
                hotels = await hotelService.GetHotels();
            }

            // Seed Rooms
            var rooms = await roomService.GetRooms();
            if (rooms.Count == 0)
            {
                foreach (var hotel in hotels)
                {
                    var room1 = new CreateRoomDto { HotelId = hotel.Id, Name = RoomTypeName.Single, BasePrice = 100 };
                    var room2 = new CreateRoomDto { HotelId = hotel.Id, Name = RoomTypeName.Double, BasePrice = 150 };
                    var room3 = new CreateRoomDto { HotelId = hotel.Id, Name = RoomTypeName.Suite, BasePrice = 250 };
                    var room4 = new CreateRoomDto { HotelId = hotel.Id, Name = RoomTypeName.Deluxe, BasePrice = 350 };
                    var room5 = new CreateRoomDto { HotelId = hotel.Id, Name = RoomTypeName.Family, BasePrice = 450 };

                    await roomService.CreateRoom(room1);
                    await roomService.CreateRoom(room2);
                    await roomService.CreateRoom(room3);
                    await roomService.CreateRoom(room4);
                    await roomService.CreateRoom(room5);
                }
            }
        }
    }
}
