using Library.GrpcContracts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddOpenApi();


builder.Services.AddGrpcClient<Library.GrpcContracts.LibraryService.LibraryServiceClient>(options =>
{
    options.Address = new Uri("https://localhost:5003"); // gRPC server from Library.Service
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Run();
