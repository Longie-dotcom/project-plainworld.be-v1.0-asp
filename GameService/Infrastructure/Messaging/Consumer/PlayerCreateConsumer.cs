using Application.Helper;
using Application.Interface.IService;
using MassTransit;
using PlainWorld.MessageBroker;

namespace Infrastructure.Messaging.Consumer
{
    public class PlayerCreateConsumer : IConsumer<UserCreateDTO>
    {
        private readonly IPlayerService _playerService;

        public PlayerCreateConsumer(
            IPlayerService playerService)
        {
            _playerService = playerService;
        }

        public async Task Consume(ConsumeContext<UserCreateDTO> context)
        {
            try
            {
                var message = context.Message;
                ServiceLogger.Logging(
                    Level.Infrastructure, $"Create new player info data: {message.Email}");
                _playerService.UserSyncCreating(message);
            }
            catch (Exception ex)
            {
                ServiceLogger.Error(
                    Level.Infrastructure, $"Failed when create new player info data: {ex.Message}");
            }
        }
    }
}
