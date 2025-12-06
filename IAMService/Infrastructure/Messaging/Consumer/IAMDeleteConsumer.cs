using Application.DTO;
using Application.Interface.IService;
using FSA.LaboratoryManagement.IAM.Patient.MessageBroker;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Consumer
{
    public class IAMDeleteConsumer : IConsumer<PatientRequestDeleteDTO>
    {
        private readonly ILogger<IAMDeleteConsumer> _logger;
        private readonly IUserService _userService;

        public IAMDeleteConsumer(ILogger<IAMDeleteConsumer> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task Consume(ConsumeContext<PatientRequestDeleteDTO> context)
        {
            var dto = context.Message;
            _logger.LogInformation("Received Patient delete for {IdentityNumber}", dto.IdentityNumber);

            var mappedDto = new IAMConsumeDeleteDTO()
            {
                PerformBy = dto.PerformBy,
                IdentityNumber = dto.IdentityNumber,
            };

            await _userService.PatientSyncDeleting(mappedDto);
        }
    }
}
