using Application.Helper;
using Application.Interface.IPublisher;
using MassTransit;
using PlainWorld.MessageBroker;

namespace Infrastructure.MessageBroker.Publisher
{
    public class EmailSendPublisher : IEmailSendPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;

        public EmailSendPublisher(
            IPublishEndpoint publishEndpoint)
        {
            _publishEndpoint = publishEndpoint;
        }

        public async Task SendEmail(EmailMessageDTO dto)
        {
            ServiceLogger.Logging(
                Level.Infrastructure, $"Publishing send email for email {dto.ToEmail}");
            await _publishEndpoint.Publish(dto);
        }
    }
}
