using JwtAuthDotNet.Entities;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthDotNet.Data
{
    public class UserDbContext(DbContextOptions<UserDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Hotel> Hotels { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Booking> Bookings { get; set; }

    }
}
