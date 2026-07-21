using CatalogService.Api.Middleware;
using CatalogService.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

var catalogConnectionString = builder.Configuration.GetConnectionString("Catalog") ?? string.Empty;
if (!builder.Environment.IsDevelopment() && string.IsNullOrWhiteSpace(catalogConnectionString))
{
    throw new InvalidOperationException("Catalog connection string must be configured outside development.");
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCatalogPersistence(builder.Configuration);
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        var origins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
            ?? ["http://localhost:3000"];

        policy.WithOrigins(origins).AllowAnyHeader().AllowAnyMethod();
    });
});

var app = builder.Build();

if (!app.Environment.IsEnvironment("Container"))
{
    app.UseHttpsRedirection();
}

app.UseMiddleware<ApiExceptionMiddleware>();
app.UseCors("Frontend");
app.MapControllers();
app.Run();
