using Application;
using Infrastructure;
using Infrastructure.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Clean Architecture Layers
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

// Request Logging Middleware
app.UseMiddleware<MongoRequestLoggingMiddleware>();

app.MapControllers();

app.Run();
