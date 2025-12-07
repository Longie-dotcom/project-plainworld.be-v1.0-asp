using PlainWorld.MessageBroker;

namespace Application.Interface.IPublisher
{
    public interface IUserUpdatePublisher
    {
        Task PublishAsync(UserUpdateRequestDTO dto);
    }
}
