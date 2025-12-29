using Application.Helper;
using Application.Interface.IPublisher;
using MassTransit;
using PlainWorld.MessageBroker;

namespace Infrastructure.MessageBroker.Publisher
{
    public class UserCreatePublisher : IUserCreatePublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public UserCreatePublisher(
            IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishAsync(UserCreateDTO dto)
        {
            ServiceLogger.Logging(
                Level.Infrastructure, $"Publishing user create for user ID: {dto.UserID}");
            await _publishEndpoint.Publish(dto);
        }
    }
}
