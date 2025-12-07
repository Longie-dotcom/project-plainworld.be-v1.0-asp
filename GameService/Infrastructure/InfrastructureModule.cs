using Application.Helper;
using Application.Interface.IGrpcClient;
using Application.Interface.IMessageBrokerPublisher;
using Domain.IRepository;
using Infrastructure.Grpc;
using Infrastructure.InfrastructureException;
using Infrastructure.Messaging.Consumer;
using Infrastructure.Messaging.Publisher;
using Infrastructure.Persistence.Configuration;
using Infrastructure.Persistence.Repository;
using Infrastructure.Temporary;
using MassTransit;
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
            ServiceLogger.Logging(
                Level.Infrastructure, "Starting Infrastructure configuration");

            // ======================
            // 1. Database
            // ======================
            try
            {
                ServiceLogger.Logging(
                    Level.Infrastructure, "Configuring GameDB connection");

                var mongoConnection = Environment.GetEnvironmentVariable("GAME_DB_CONNECTION");

                if (string.IsNullOrEmpty(mongoConnection))
                {
                    ServiceLogger.Logging(
                        Level.Infrastructure, "Missing environment variable: GAME_DB_CONNECTION");
                    throw new DatabaseConnectionException(
                        "Failed to configure GameDB connection.");
                }

                services.AddSingleton(sp => new GameDBContext(
                    mongoConnection,
                    "GameDB"));

                // Register repositories (MongoDB-based)
                services.AddScoped<IPlayerRepository, PlayerRepository>();
                services.AddScoped<IUnitOfWork, UnitOfWork>();

                ServiceLogger.Logging(
                    Level.Infrastructure, "GameDB and document repositories configured successfully.");
            }
            catch (Exception ex)
            {
                ServiceLogger.Logging(
                    Level.Infrastructure, $"Failed to configure MongoDB connection: {ex.Message}");
                throw new DatabaseConnectionException(
                    "Failed to configure MongoDB connection.");
            }

            //======================
            //2.RabbitMQ
            //======================
            try
            {
                ServiceLogger.Logging(
                    Level.Infrastructure, "Configuring RabbitMQ connection");

                services.AddMassTransit(x =>
                {
                    // Add all consumers for this service
                    x.AddConsumer<Consumer>();

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
                            ServiceLogger.Logging(
                                Level.Infrastructure, "Missing RabbitMQ environment variables");
                            throw new MessagingConnectionException("Failed to configure message broker.");
                        }

                        cfg.Host(rabbitHost, "/", h =>
                        {
                            h.Username(rabbitUser);
                            h.Password(rabbitPassword);
                        });

                        cfg.ReceiveEndpoint("consumer", e =>
                        {
                            e.ConfigureConsumer<Consumer>(context);
                        });
                    });
                });

                services.AddScoped<IPublisher, Publisher>();

                ServiceLogger.Logging(
                    Level.Infrastructure, "RabbitMQ successfully configured.");
            }
            catch (Exception ex)
            {
                ServiceLogger.Logging(
                    Level.Infrastructure, $"RabbitMQ configuration failed: {ex.Message}");
                throw new MessagingConnectionException(
                    "Failed to configure RabbitMQ infrastructure.");
            }

            //======================
            //3.gRPC Clients
            //======================
            try
            {
                ServiceLogger.Logging(
                    Level.Infrastructure, "Configuring gRPC connection.");

                services.AddScoped<IClient, Client>();

                ServiceLogger.Logging(
                    Level.Infrastructure, "gRPC configured successfully.");
            }
            catch (Exception ex)
            {
                ServiceLogger.Logging(
                    Level.Infrastructure, $"gRPC configuration failed: {ex.Message}");
                throw new GrpcCommunicationException(
                    "Failed to configure gRPC client.");
            }

            //======================
            //4.In memory
            //======================
            try
            {
                ServiceLogger.Logging(
                    Level.Infrastructure, "Configuring in-memory game state.");

                services.AddSingleton<IInMemoryGameState, InMemoryGameState>();

                ServiceLogger.Logging(
                    Level.Infrastructure, "In-memory game state configured successfully.");
            }
            catch (Exception ex)
            {
                ServiceLogger.Logging(
                    Level.Infrastructure, $"gRPC configuration failed: {ex.Message}");
                throw new InMemoryGameStateException(
                    "Failed to configure in-memory game state.");
            }

            return services;
        }
        #endregion
    }
}
