
using PlainWorld.MessageBroker;

namespace Application.Interface.IPublisher
{
    public interface IEmailSendPublisher
    {
        Task SendEmail(EmailMessageDTO dto);
    }
}
