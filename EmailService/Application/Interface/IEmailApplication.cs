using PlainWorld.MessageBroker;

namespace Application.Interface
{
    public interface IEmailApplication
    {
        List<EmailMessageDTO> GetEmailMessages();
        Task PublishEmail(EmailMessageDTO message);
        Task PublishEmailByIdentityNumber(UserIDMessageDTO message);
    }
}
