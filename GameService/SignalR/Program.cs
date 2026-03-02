using Application;
using Application.Interface.GameEventPublisher;
using DotNetEnv;
using Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SignalR;
using System.Text;

Env.Load(); // load .env variables

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// Authentication (JWT)
// -----------------------------
var jwtSecret = Env.GetString("JWT_SECRET_KEY");
var jwtIssuer = Env.GetString("JWT_ISSUER");
var jwtAudience = Env.GetString("JWT_AUDIENCE");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret)
            ),

            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) &&
                    path.StartsWithSegments("/hubs/game"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization();

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
// Game Event Publisher
// -----------------------------
builder.Services.AddSingleton<IGameEventPublisher, GameEventPublisher>();

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
app.UseAuthentication();
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
