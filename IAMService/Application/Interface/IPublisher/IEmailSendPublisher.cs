using FSA.LaboratoryManagement.EmailMessage;

namespace Application.Interface.IPublisher
{
    public interface IEmailSendPublisher
    {
        Task SendEmail(EmailMessageDTO dto);
    }
}
