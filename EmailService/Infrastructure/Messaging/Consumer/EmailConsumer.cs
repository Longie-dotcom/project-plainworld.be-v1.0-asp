using Application.Interface;
using FSA.LaboratoryManagement.EmailMessage;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Consumer
{
    public class EmailConsumer : IConsumer<EmailMessageDTO>
    {
        private readonly ILogger<EmailConsumer> _logger;
        private readonly IEmailApplication _emailApplication;

        public EmailConsumer(ILogger<EmailConsumer> logger, IEmailApplication emailApplication)
        {
            _logger = logger;
            _emailApplication = emailApplication;
        }

        public async Task Consume(ConsumeContext<EmailMessageDTO> context)
        {
            var dto = context.Message;
            _logger.LogInformation($"Received email sending request for email: {dto.ToEmail}");

            await _emailApplication.PublishEmail(dto);
        }
    }
}
