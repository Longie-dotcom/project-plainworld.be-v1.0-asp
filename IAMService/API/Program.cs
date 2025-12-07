using API.GrpcService;
using API.Middleware;
using Application;
using DotNetEnv;
using Infrastructure;
using Infrastructure.Persistence.Seed;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PlainWorld.Authorization;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// -----------------------------
// Core setup
// -----------------------------
builder.Services.AddGrpc();
builder.Services.AddInfrastructure();
builder.Services.AddApplication();
builder.Services.AddControllers();
// gRPC Service
builder.Services.AddScoped<IAMGrpcService>();

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
    options.ListenAnyIP(5001, listenOptions =>
    {
        listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
    });

    // REST endpoint — uses standard HTTP/1.1
    options.ListenAnyIP(5000, listenOptions =>
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
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<IAMDBContext>();

    var retries = 5;
    for (int i = 0; i < retries; i++)
    {
        try
        {
            // Create DB if it doesn’t exist and apply all migrations
            await db.Database.MigrateAsync();

            // Always seed after migration
            await RolePrivilegeSeeder.SeedAsync(db);

            Console.WriteLine("Database migrated and seeded successfully.");
            break;
        }
        catch (SqlException)
        {
            Console.WriteLine($"Database not ready, retrying in 5s... ({i + 1}/{retries})");
            await Task.Delay(5000);
            if (i == retries - 1) throw;
        }
    }
}

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
app.MapGrpcService<IAMGrpcService>();

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
app.MapGet("/health", () => Results.Ok("IAM Service running"));

// -----------------------------
// Run
// -----------------------------
app.Run();
