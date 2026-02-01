using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using WebService.Data;
using WebService.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgresConnection")));

// MongoDB
builder.Services.AddSingleton<IMongoClient>(sp => 
    new MongoClient(builder.Configuration.GetConnectionString("MongoConnection")));

// Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
});

builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthorization();

// Request Logging Middleware
app.UseMiddleware<MongoRequestLoggingMiddleware>();

app.MapControllers();

app.Run();
