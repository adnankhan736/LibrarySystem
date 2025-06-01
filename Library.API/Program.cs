using Library.GrpcContracts;

namespace Library.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = CreateWebApplicationBuilder(args);
        var app = BuildApp(builder);
        app.Run();
    }

    public static WebApplicationBuilder CreateWebApplicationBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container
        builder.Services.AddControllers()
              .AddNewtonsoftJson(options =>
              {
                  options.SerializerSettings.ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver();
              });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddGrpcClient<LibraryService.LibraryServiceClient>(options =>
        {
            var grpcAddress = builder.Configuration.GetValue<string>("GrpcSettings:LibraryServiceUrl")
                              ?? "https://localhost:7179";
            options.Address = new Uri(grpcAddress);
        });

        return builder;
    }

    public static WebApplication BuildApp(WebApplicationBuilder builder)
    {
        var app = builder.Build();

        // Configure the HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.MapControllers();

        return app;
    }
}
