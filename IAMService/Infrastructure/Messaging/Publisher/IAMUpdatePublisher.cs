using Application.DTO;
using Application.Interface.IPublisher;
using FSA.LaboratoryManagement.IAM.Patient.MessageBroker;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MessageBroker.Publisher
{
    public class IAMUpdatePublisher : IIAMUpdatePublisher
    {
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly ILogger<IAMUpdatePublisher> _logger;

        public IAMUpdatePublisher(
            IPublishEndpoint publishEndpoint, ILogger<IAMUpdatePublisher> logger)
        {
            _publishEndpoint = publishEndpoint;
            _logger = logger;
        }

        public async Task PublishAsync(IAMRequestUpdateMBDTO dto)
        {
            _logger.LogInformation("Publishing IAM update for {IdentityNumber}", dto.IdentityNumber);

            var mappedDto = new IAMRequestUpdateDTO()
            { 
                IdentityNumber = dto.IdentityNumber,
                Address = dto.Address,
                DateOfBirth = DateOnly.FromDateTime(dto.DateOfBirth.Value),
                FullName = dto.FullName,
                Gender = dto.Gender,
                PhoneNumber = dto.PhoneNumber
            };

            await _publishEndpoint.Publish(mappedDto);
        }
    }
}
