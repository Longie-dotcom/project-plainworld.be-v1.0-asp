using API.Middleware;
using Application;
using DotNetEnv;
using FSA.LaboratoryManagement.Authorization;
using Infrastructure;
using Microsoft.Extensions.Logging.Console;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// Logging
// -----------------------------
var loggerFactory = LoggerFactory.Create(logging =>
{
    logging.ClearProviders();
    logging.AddSimpleConsole(options =>
    {
        options.TimestampFormat = "[HH:mm:ss] ";
        options.ColorBehavior = LoggerColorBehavior.Enabled;
    });
    logging.SetMinimumLevel(LogLevel.Information);
});

// -----------------------------
// Core setup
// -----------------------------
builder.Services.AddGrpc();
builder.Services.AddInfrastructure(loggerFactory);
builder.Services.AddApplication();
builder.Services.AddControllers();

// -----------------------------
// Swagger
// -----------------------------
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// -----------------------------
// Kestrel
// -----------------------------
builder.WebHost.ConfigureKestrel(options =>
{
    // gRPC endpoint — requires HTTP/2
    options.ListenAnyIP(5201, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });

    // REST endpoint — uses standard HTTP/1.1
    options.ListenAnyIP(5200, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
    });
});

// -----------------------------
// Privilege Authorization
// -----------------------------
builder.Services.AddPrivilegeAuthorization();

var app = builder.Build();

// -----------------------------
// DB migration & seeding
// -----------------------------


// -----------------------------
// Swagger UI
// -----------------------------
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "IAM Service API v1");
    c.RoutePrefix = string.Empty;
});

// -----------------------------
// Map Grpc
// -----------------------------

// -----------------------------
// Middlewares
// -----------------------------
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<SuccessResponseMiddleware>();
app.UseMiddleware<TokenClaimsMiddleware>();

app.UseRouting();
app.UseAuthorization();

// -----------------------------
// Controllers & health endpoint
// -----------------------------
app.MapControllers();
app.MapGet("/health", () => Results.Ok("Email Service running"));

// -----------------------------
// Run
// -----------------------------
app.Run();
