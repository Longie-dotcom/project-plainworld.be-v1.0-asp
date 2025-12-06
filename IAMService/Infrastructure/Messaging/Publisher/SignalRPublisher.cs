using Application.Interface.IPublisher;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MessageBroker.Publisher
{
    public class SignalRPublisher : ISignalRPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<EmailSendPublisher> _logger;

        public SignalRPublisher(
            IPublishEndpoint publishEndpoint, ILogger<EmailSendPublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task PublishEnvelop(SignalREnvelope.SignalREnvelope dto)
        {
            _logger.LogInformation($"Publishing signalR for realtime function {dto.Method}");
            await _publishEndpoint.Publish(dto);
        }
    }
}
