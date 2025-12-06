using FSA.LaboratoryManagement.EmailMessage;

namespace Application.Interface
{
    public interface IEmailRepository
    {
        List<EmailMessageDTO> GetAllEmails();
        void AddEmail(EmailMessageDTO emailMessageDTO);
    }
}
