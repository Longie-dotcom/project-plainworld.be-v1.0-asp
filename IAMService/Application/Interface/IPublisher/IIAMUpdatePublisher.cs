using Application.DTO;

namespace Application.Interface.IPublisher
{
    public interface IIAMUpdatePublisher
    {
        Task PublishAsync(IAMRequestUpdateMBDTO dto);
    }
}
