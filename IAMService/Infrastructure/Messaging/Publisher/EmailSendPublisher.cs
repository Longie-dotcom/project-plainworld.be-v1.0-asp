using Application.Interface.IPublisher;
using FSA.LaboratoryManagement.EmailMessage;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MessageBroker.Publisher
{
    public class EmailSendPublisher : IEmailSendPublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<EmailSendPublisher> _logger;

        public EmailSendPublisher(
            IPublishEndpoint publishEndpoint, ILogger<EmailSendPublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task SendEmail(EmailMessageDTO dto)
        {
            _logger.LogInformation($"Publishing send email for email {dto.ToEmail}");
            await _publishEndpoint.Publish(dto);
        }
    }
}
