using Application.Interface;
using FSA.LaboratoryManagement.EmailMessage;

namespace Infrastructure.Persistence
{
    public class EmailRepository : IEmailRepository
    {
        public void AddEmail(EmailMessageDTO emailMessageDTO)
        {
            EmailDBMemo.EmailMessages.Add(emailMessageDTO);
        }

        public List<EmailMessageDTO> GetAllEmails()
        {
            return EmailDBMemo.EmailMessages;
        }
    }
}
