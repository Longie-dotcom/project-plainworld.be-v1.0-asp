using FSA.LaboratoryManagement.EmailMessage;

namespace Application.Interface
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailMessageDTO email);
    }
}
