using Library.GrpcContracts;
using Grpc.Net.ClientFactory;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Library.API.SystemTests.Mocks;

namespace Library.API.SystemTests
{
    public class TestWebApplicationFactory : WebApplicationFactory<Program>
    {
        public Mock<LibraryService.LibraryServiceClient> GrpcClientMock { get; private set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Remove existing gRPC client registration
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(LibraryService.LibraryServiceClient));
                if (descriptor != null)
                    services.Remove(descriptor);

                // Create and register mocked gRPC client with default setup
                GrpcClientMock = GrpcMockFactory.CreateMockClient();
                services.AddSingleton(GrpcClientMock.Object);
            });
        }
    }
}
