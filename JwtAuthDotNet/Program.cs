using JwtAuthDotNet.Data;
using JwtAuthDotNet.Services.Interfaces;
using JwtAuthDotNet.Services.Implementations;
using JwtAuthDotNet.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Microsoft.IdentityModel.Tokens;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddDbContext<UserDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("userDatabase"));
}
    );
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UserController>();
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

    }
    );
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
