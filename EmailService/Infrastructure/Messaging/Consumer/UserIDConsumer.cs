using Application.Interface;
using MassTransit;
using Microsoft.Extensions.Logging;
using PlainWorld.MessageBroker;

namespace Infrastructure.Messaging.Consumer
{
    public class UserIDConsumer : IConsumer<UserIDMessageDTO>
    {
        private readonly ILogger<UserIDConsumer> _logger;
        private readonly IEmailApplication _emailApplication;

        public UserIDConsumer(
            ILogger<UserIDConsumer> logger, 
            IEmailApplication emailApplication)
        {
            _logger = logger;
            _emailApplication = emailApplication;
        }

        public async Task Consume(ConsumeContext<UserIDMessageDTO> context)
        {
            try
            {
                var dto = context.Message;
                _logger.LogInformation(
                    $"Received sending email request for user ID: {dto.ToUserID}");
                await _emailApplication.PublishEmailByIdentityNumber(dto);
            }
            catch (Exception ex)
            {
                _logger.LogInformation(
                    $"Failed when received sending email request for user ID: {ex.Message}");
            }
        }
    }
}
