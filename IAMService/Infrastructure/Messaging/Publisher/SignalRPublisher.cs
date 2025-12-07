using Application.Helper;
using Application.Interface.IPublisher;
using MassTransit;

namespace Infrastructure.MessageBroker.Publisher
{
    public class SignalRPublisher : ISignalRPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public SignalRPublisher(
            IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task PublishEnvelop(SignalREnvelope.SignalREnvelope dto)
        {
            ServiceLogger.Logging(
                Level.Infrastructure, $"Publishing signalR for realtime function {dto.Method}");
            await _publishEndpoint.Publish(dto);
        }
    }
}
