using Library.GrpcContracts;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers(); // Required for Web API controllers
builder.Services.AddEndpointsApiExplorer(); // Required for Swagger
builder.Services.AddSwaggerGen(); // Enables Swagger UI

builder.Services.AddGrpcClient<LibraryService.LibraryServiceClient>(options =>
{
    options.Address = new Uri("https://localhost:7179"); // gRPC server from Library.Service
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers(); // Maps controller endpoints like /api/library/...

app.Run();
