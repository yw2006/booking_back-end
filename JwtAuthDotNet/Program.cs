using JwtAuthDotNet.Data;
using JwtAuthDotNet.Services.Interfaces;
using JwtAuthDotNet.Services.Implementations;
using JwtAuthDotNet.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.FileProviders;
using JwtAuthDotNet.testing;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()   // ✅ allow all origins
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("userDatabase"));
});

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IHotelService, HotelService>();
builder.Services.AddScoped<IRoomService, RoomService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<IHotelSearchService, HotelSearchService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["AppSettings:issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["AppSettings:audience"],
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            System.Text.Encoding.UTF8.GetBytes(
                builder.Configuration.GetValue<string>("AppSettings:token")!)),
        ValidateIssuerSigningKey = true,

    });

builder.Services.AddAuthorization();


var app = builder.Build();
app.UseCors("AllowAll"); // ✅ apply CORS globally
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var hotelService = services.GetRequiredService<IHotelService>();
//    var roomService = services.GetRequiredService<IRoomService>();
//    await DbSeeder.SeedDatabase(hotelService, roomService);
//}

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
        Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "images")),
    RequestPath = "/images"
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
