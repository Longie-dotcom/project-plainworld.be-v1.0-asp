using Application;
using DotNetEnv;
using Infrastructure;
using SignalR;

Env.Load(); // load .env variables

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// CORS
// -----------------------------
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .SetIsOriginAllowed(_ => true);
    });
});

// -----------------------------
// Infrastructure, Application (MassTransit + RabbitMQ)
// -----------------------------
builder.Services.AddInfrastructure();
builder.Services.AddApplication();

// -----------------------------
// SignalR
// -----------------------------
builder.Services.AddSignalR();

// -----------------------------
// Controllers
// -----------------------------
builder.Services.AddControllers();

// -----------------------------
// Swagger
// -----------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -----------------------------
// Kestrel Configuration
// -----------------------------
builder.WebHost.ConfigureKestrel(options =>
{
    // HTTP endpoint for API and SignalR
    options.ListenAnyIP(5020, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
    });

    // HTTPS endpoint (optional, can disable if not needed)
    options.ListenAnyIP(5021, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1AndHttp2;
    });
});

// -----------------------------
// Build
// -----------------------------
var app = builder.Build();

// -----------------------------
// Swagger UI
// -----------------------------
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SignalR Central Service API v1");
    c.RoutePrefix = string.Empty; // Swagger at root URL
});

// -----------------------------
// Middlewares
// -----------------------------
app.UseCors();
app.UseAuthorization();

// -----------------------------
// Map SignalR hubs
// -----------------------------
app.MapHub<GameHub>("/hubs/game");

// -----------------------------
// Map controllers
// -----------------------------
app.MapControllers();

// -----------------------------
// Health check
// -----------------------------
app.MapGet("/health", () => Results.Ok("SignalR Central Service running"));

// -----------------------------
// Run
// -----------------------------
app.Run();
