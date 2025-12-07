using Application.Interface;
using MassTransit;
using Microsoft.Extensions.Logging;
using PlainWorld.MessageBroker;

namespace Infrastructure.Messaging.Consumer
{
    public class EmailConsumer : IConsumer<EmailMessageDTO>
    {
        private readonly ILogger<EmailConsumer> _logger;
        private readonly IEmailApplication _emailApplication;

        public EmailConsumer(
            ILogger<EmailConsumer> logger, 
            IEmailApplication emailApplication)
        {
            _logger = logger;
            _emailApplication = emailApplication;
        }

        public async Task Consume(ConsumeContext<EmailMessageDTO> context)
        {
            try
            {
                var dto = context.Message;
                _logger.LogInformation(
                    $"Received email sending request for email: {dto.ToEmail}");
                await _emailApplication.PublishEmail(dto);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(
                    $"Failed when received email sending request for email: {ex.Message}");
            }
        }
    }
}
