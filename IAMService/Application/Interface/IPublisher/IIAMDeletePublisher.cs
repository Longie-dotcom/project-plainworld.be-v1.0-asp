using Application.DTO;

namespace Application.Interface.IPublisher
{
    public interface IIAMDeletePublisher
    {
        Task PublishAsync(IAMRequestDeleteMBDTO dto);
    }
}