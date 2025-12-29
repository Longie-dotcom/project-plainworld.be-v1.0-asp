using PlainWorld.MessageBroker;

namespace Application.Interface.IPublisher
{
    public interface IUserCreatePublisher
    {
        Task PublishAsync(UserCreateDTO dto);
    }
}
