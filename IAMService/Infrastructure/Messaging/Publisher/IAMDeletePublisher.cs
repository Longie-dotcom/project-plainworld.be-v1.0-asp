using Application.DTO;
using Application.Interface.IPublisher;
using FSA.LaboratoryManagement.IAM.Patient.MessageBroker;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MessageBroker.Publisher
{
    public class IAMDeletePublisher : IIAMDeletePublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<IAMDeletePublisher> _logger;

        public IAMDeletePublisher(IPublishEndpoint publishEndpoint, ILogger<IAMDeletePublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task PublishAsync(IAMRequestDeleteMBDTO dto)
        {
            _logger.LogInformation("Publishing IAM delete for {IdentityNumber}", dto.IdentityNumber);
            
            var mappedDto = new IAMRequestDeleteDTO { IdentityNumber = dto.IdentityNumber };

            await _publishEndpoint.Publish(mappedDto);
        }
    }
}
