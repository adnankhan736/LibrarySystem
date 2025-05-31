using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Library.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;
using Library.GrpcContracts;
using GrpcService = Library.Service;

public class GrpcTestBase : IAsyncLifetime
{
    protected GrpcChannel Channel { get; private set; }
    protected LibraryService.LibraryServiceClient Client { get; private set; }
    private WebApplicationFactory<GrpcService.Program> _factory;

    public async Task InitializeAsync()
    {
        _factory = new WebApplicationFactory<GrpcService.Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, config) =>
                {
                    // Override connection string for tests
                    config.AddInMemoryCollection(new Dictionary<string, string>
                    {
                        ["ConnectionStrings:DefaultConnection"] =
                            @"Server=(localdb)\mssqllocaldb;Database=LibraryTestDb;Trusted_Connection=True;MultipleActiveResultSets=true"
                    });
                });
            });
        var baseAddress = "http://localhost:5023";
        var httpClient = _factory.CreateDefaultClient(new Uri(baseAddress));
        Channel = GrpcChannel.ForAddress(baseAddress, new GrpcChannelOptions { HttpClient = httpClient });
        Client = new LibraryService.LibraryServiceClient(Channel);

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

        // Clean database before test run
        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();

    }

    public async Task DisposeAsync()
    {
        if (_factory != null)
        {
            await using var scope = _factory.Services.CreateAsyncScope();
            var db = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();
            await db.Database.EnsureDeletedAsync();
            _factory.Dispose();
        }
    }
}
