using Application.DTO;
using Application.Interface.IService;
using FSA.LaboratoryManagement.IAM.Patient.MessageBroker;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Messaging.Consumer
{
    public class IAMUpdateConsumer : IConsumer<PatientRequestUpdateDTO>
    {
        private readonly ILogger<IAMUpdateConsumer> _logger;
        private readonly IUserService _userService;

        public IAMUpdateConsumer(
            ILogger<IAMUpdateConsumer> logger, IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task Consume(ConsumeContext<PatientRequestUpdateDTO> context)
        {
            var dto = context.Message;
            _logger.LogInformation("Received Patient update for {IdentityNumber}", dto.IdentityNumber);

            var mappedDto = new IAMConsumeUpdateDTO()
            {
                Address = dto.Address,
                DateOfBirth = dto.DateOfBirth,
                FullName = dto.FullName,
                Gender = dto.Gender,
                IdentityNumber = dto.IdentityNumber,
                PerformBy = dto.PerformBy,
                PhoneNumber = dto.PhoneNumber
            };

            await _userService.PatientSyncUpdating(mappedDto);
        }
    }
}
