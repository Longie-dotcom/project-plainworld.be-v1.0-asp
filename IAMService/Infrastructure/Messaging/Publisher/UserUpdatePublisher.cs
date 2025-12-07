using Application.Helper;
using Application.Interface.IPublisher;
using MassTransit;
using PlainWorld.MessageBroker;

namespace Infrastructure.MessageBroker.Publisher
{
    public class UserUpdatePublisher : IUserUpdatePublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public UserUpdatePublisher(
            IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishAsync(UserUpdateRequestDTO dto)
        {
            ServiceLogger.Logging(
                Level.Infrastructure, $"Publishing user update for user ID: {dto.UserID}");
            await _publishEndpoint.Publish(dto);
        }
    }
}
