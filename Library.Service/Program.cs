using Library.Application.Interfaces;
using Library.Application.Services;
using Library.Service.Services;
using Microsoft.EntityFrameworkCore;
using Library.Infrastructure.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

// Get connection string from appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");


// Register DbContext
builder.Services.AddDbContext<LibraryDbContext>(options =>
    options.UseSqlServer(connectionString));

// Add services
builder.Services.AddGrpc();
builder.Services.AddScoped<ILibraryCoreService, LibraryCoreService>();

var app = builder.Build();

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
    db.Database.Migrate(); // Apply migrations
}

// Map gRPC and run
app.MapGrpcService<LibraryGrpcService>();
app.MapGet("/", () => "Use a gRPC client: https://go.microsoft.com/fwlink/?linkid=2086909");

app.Run();
