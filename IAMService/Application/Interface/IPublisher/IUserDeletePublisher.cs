using PlainWorld.MessageBroker;

namespace Application.Interface.IPublisher
{
    public interface IUserDeletePublisher
    {
        Task PublishAsync(UserUpdateRequestDTO dto);
    }
}