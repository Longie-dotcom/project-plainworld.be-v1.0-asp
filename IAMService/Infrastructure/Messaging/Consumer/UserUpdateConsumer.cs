using Application.Helper;
using Application.Interface.IService;
using MassTransit;
using PlainWorld.MessageBroker;

namespace Infrastructure.Messaging.Consumer
{
    public class UserUpdateConsumer : IConsumer<UserUpdateRequestDTO>
    {
        private readonly IUserService _userService;

        public UserUpdateConsumer(
            IUserService userService)
        {
            _userService = userService;
        }

        public async Task Consume(ConsumeContext<UserUpdateRequestDTO> context)
        {
            try
            {
                var message = context.Message;
                ServiceLogger.Logging(
                    Level.Infrastructure, $"Sync up user data: {message.Email}");
                await _userService.UserSyncUpdating(message);
            }
            catch (Exception ex)
            {
                ServiceLogger.Error(
                    Level.Infrastructure, $"Failed when sync up user data: {ex.Message}");
            }
        }
    }
}
