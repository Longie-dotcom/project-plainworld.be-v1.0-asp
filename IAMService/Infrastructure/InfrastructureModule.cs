using Application.Helper;
using Application.Interface.IGrpcClient;
using Application.Interface.IPublisher;
using Application.Interface.IService;
using Domain.IRepository;
using Infrastructure.ExternalService.Security;
using Infrastructure.Grpc;
using Infrastructure.InfrastructureException;
using Infrastructure.MessageBroker.Publisher;
using Infrastructure.Messaging.Consumer;
using Infrastructure.Persistence.Repository;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class InfrastructureModule
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        #region Methods
        public static IServiceCollection AddInfrastructure(
                this IServiceCollection services)
        {
            ServiceLogger.Warning(
                Level.Infrastructure, "Starting Infrastructure configuration");

            // ======================
            // 1. Database
            // ======================
            try
            {
                ServiceLogger.Warning(
                    Level.Infrastructure, "Configuring SQL Server database connection");

                // Configure the database connection
                var connectionString = Environment.GetEnvironmentVariable("IAM_DB_CONNECTION");
                if (string.IsNullOrEmpty(connectionString))
                {
                    ServiceLogger.Error(
                        Level.Infrastructure, "Missing environment variable: IAM_DB_CONNECTION");
                    throw new DatabaseConnectionException(
                        "Failed to configure IAM database.");
                }

                services.AddDbContext<IAMDBContext>(options =>
                    options.UseSqlServer(connectionString));

                // Register repositories + Unit of Work + Mapper

                services.AddScoped<IUserRepository, UserRepository>();
                services.AddScoped<IRoleRepository, RoleRepository>();
                services.AddScoped<IAuditLogRepository, AuditLogRepository>();
                services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
                services.AddScoped<IPrivilegeRepository, PrivilegeRepository>();

                services.AddScoped<IUnitOfWork, UnitOfWork>();

                ServiceLogger.Logging(
                    Level.Infrastructure, "Database and repositories configured successfully.");
            }
            catch (Exception ex)
            {
                ServiceLogger.Error(
                    Level.Infrastructure, $"Failed to configure IAM database: {ex.Message}");
                throw new DatabaseConnectionException(
                    "Failed to configure IAM database.");
            }

             //======================
             //2.RabbitMQ
             //======================
            try
            {
                ServiceLogger.Warning(
                    Level.Infrastructure, "Configuring RabbitMQ connection");

                services.AddMassTransit(x =>
                {
                    // Add all consumers for this service
                    x.AddConsumer<UserUpdateConsumer>();

                    x.UsingRabbitMq((context, cfg) =>
                    {
                        var rabbitHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST");
                        var rabbitUser = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER");
                        var rabbitPassword = Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS");

                        if (
                        string.IsNullOrEmpty(rabbitHost) 
                        || string.IsNullOrEmpty(rabbitUser) 
                        || string.IsNullOrEmpty(rabbitPassword))
                        {
                            ServiceLogger.Error(
                                Level.Infrastructure, "Missing RabbitMQ environment variables");
                            throw new MessagingConnectionException("Failed to configure message broker.");
                        }

                        cfg.Host(rabbitHost, "/", h =>
                        {
                            h.Username(rabbitUser);
                            h.Password(rabbitPassword);
                        });

                        cfg.ReceiveEndpoint("iam_consumer", e =>
                        {
                            e.ConfigureConsumer<UserUpdateConsumer>(context);
                        });
                    });
                });

                services.AddScoped<IUserUpdatePublisher, UserUpdatePublisher>();
                services.AddScoped<IUserDeletePublisher, UserDeletePublisher>();
                services.AddScoped<IEmailSendPublisher, EmailSendPublisher>();
                services.AddScoped<ISignalRPublisher, SignalRPublisher>();

                ServiceLogger.Logging(
                    Level.Infrastructure, "RabbitMQ successfully configured.");
            }
            catch (Exception ex)
            {
                ServiceLogger.Error(
                    Level.Infrastructure, $"RabbitMQ configuration failed: {ex.Message}");
                throw new MessagingConnectionException(
                    "Failed to configure RabbitMQ infrastructure.");
            }

             //======================
             //3.gRPC Clients
             //======================
            try
            {
                ServiceLogger.Warning(
                    Level.Infrastructure, $"Configuring gRPC connection.");

                services.AddScoped<IGrpcClient, GrpcClient>();

                ServiceLogger.Logging(
                    Level.Infrastructure, "gRPC configured successfully.");
            }
            catch (Exception ex)
            {
                ServiceLogger.Error(
                    Level.Infrastructure, $"gRPC configuration failed: {ex.Message}");
                throw new GrpcCommunicationException(
                    "Failed to configure gRPC client.");
            }

             //======================
             //4.JWT Token
             //======================
            try
            {
                ServiceLogger.Warning(
                    Level.Infrastructure, "Configuring JWT token");

                var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
                var issuer = Environment.GetEnvironmentVariable("JWT_ISSUER");
                var audience = Environment.GetEnvironmentVariable("JWT_AUDIENCE");
                var expiryStr = Environment.GetEnvironmentVariable("JWT_EXPIRY_MINUTES");

                if (string.IsNullOrEmpty(secretKey))
                {
                    ServiceLogger.Error(
                        Level.Infrastructure, "Missing environment variable: JWT_SECRET_KEY");
                    throw new InvalidJWTTokenException(
                        "Failed to configure JWT token service.");
                }
                if (string.IsNullOrEmpty(issuer))
                {
                    ServiceLogger.Error(
                        Level.Infrastructure, "Missing environment variable: JWT_ISSUER");
                    throw new InvalidJWTTokenException(
                        "Failed to configure JWT token service.");
                }
                if (string.IsNullOrEmpty(audience))
                {
                    ServiceLogger.Error(
                        Level.Infrastructure, "Missing environment variable: JWT_AUDIENCE");
                    throw new InvalidJWTTokenException(
                        "Failed to configure JWT token service.");
                }
                if (string.IsNullOrEmpty(expiryStr))
                {
                    ServiceLogger.Error(
                        Level.Infrastructure, "Missing environment variable: JWT_EXPIRY_MINUTES");
                    throw new InvalidJWTTokenException(
                        "Failed to configure JWT token service.");
                }

                var expiryMinutes = int.TryParse(expiryStr, out var exp) ? exp : 60;

                services.AddSingleton<ITokenService>(sp =>
                    new JWTTokenService(secretKey, issuer, audience, expiryMinutes));

                ServiceLogger.Logging(
                    Level.Infrastructure, "JWT token successfully configured.");
            }
            catch (Exception ex)
            {
                ServiceLogger.Error(
                    Level.Infrastructure, $"JWT token configuration failed: {ex.Message}");
                throw new InvalidJWTTokenException(
                    "Failed to configure JWT token service.");
            }

            return services;
        }
        #endregion
    }
}
