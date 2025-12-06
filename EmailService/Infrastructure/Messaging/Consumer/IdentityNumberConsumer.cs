using Application.Interface;
using FSA.LaboratoryManagement.EmailMessage;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Consumer
{
    public class IdentityNumberConsumer : IConsumer<IdentityNumberMessageDTO>
    {
        private readonly ILogger<IdentityNumberConsumer> _logger;
        private readonly IEmailApplication _emailApplication;

        public IdentityNumberConsumer(ILogger<IdentityNumberConsumer> logger, IEmailApplication emailApplication)
        {
            _logger = logger;
            _emailApplication = emailApplication;
        }

        public async Task Consume(ConsumeContext<IdentityNumberMessageDTO> context)
        {
            var dto = context.Message;
            _logger.LogInformation($"Received sending email request for identity: {dto.ToIdentityNumber}");

            await _emailApplication.PublishEmailByIdentityNumber(dto);
        }
    }
}
