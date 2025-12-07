using PlainWorld.MessageBroker;

namespace Application.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessageDTO email);
    }
}
