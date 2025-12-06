using FSA.LaboratoryManagement.EmailMessage;

namespace Application.Interface
{
    public interface IEmailApplication
    {
        List<EmailMessageDTO> GetEmailMessages();
        Task PublishEmail(EmailMessageDTO message);
        Task PublishEmailByIdentityNumber(IdentityNumberMessageDTO message);
    }
}
