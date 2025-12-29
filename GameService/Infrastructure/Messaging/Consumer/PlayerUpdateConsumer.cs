using Application.Helper;
using Application.Interface.IService;
using MassTransit;
using PlainWorld.MessageBroker;

namespace Infrastructure.Messaging.Consumer
{
    public class PlayerUpdateConsumer : IConsumer<UserUpdateDTO>
    {
        private readonly IPlayerService _playerService;

        public PlayerUpdateConsumer(
            IPlayerService playerService)
        {
            _playerService = playerService;
        }

        public async Task Consume(ConsumeContext<UserUpdateDTO> context)
        {
            try
            {
                var message = context.Message;
                ServiceLogger.Logging(
                    Level.Infrastructure, $"Sync up player info data: {message.Email}");
                await _playerService.UserSyncUpdating(message);
            }
            catch (Exception ex)
            {
                ServiceLogger.Error(
                    Level.Infrastructure, $"Failed when sync up player info data: {ex.Message}");
            }
        }
    }
}
