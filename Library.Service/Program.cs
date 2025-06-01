using Library.Application.Interfaces;
using Library.Application.Services;
using Library.Service.Services;
using Microsoft.EntityFrameworkCore;
using Library.Infrastructure.Data;

namespace Library.Service;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
            ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

        builder.Services.AddDbContext<LibraryDbContext>(options =>
            options.UseSqlServer(connectionString));

        builder.Services.AddGrpc();
        builder.Services.AddScoped<ILibraryCoreService, LibraryCoreService>();

        var app = builder.Build();

        using (var scope = app.Services.CreateScope())
        {
            var env = scope.ServiceProvider.GetRequiredService<IHostEnvironment>();
            if (!env.IsEnvironment("Testing")) // Skip migration when running tests
            {
                var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
                db.Database.Migrate();
            }
        }

        app.MapGrpcService<LibraryGrpcService>();
        app.MapGet("/", () => "Use a gRPC client: https://go.microsoft.com/fwlink/?linkid=2086909");

        app.Run();
    }
}
