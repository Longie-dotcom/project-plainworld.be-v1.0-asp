using Application.Interface;
using AutoMapper;
using FSA.LaboratoryManagement.EmailMessage;
using System.Net.Mail;

namespace Application.Service
{
    public class EmailApplication : IEmailApplication
    {
        #region Attributes
        private readonly IEmailService emailService;
        private readonly IIAMClient iAMClient;
        private readonly IEmailRepository emailRepository;
        #endregion

        #region Properties
        #endregion

        public EmailApplication(
            IEmailService emailService,
            IIAMClient iAMClient,
            IEmailRepository emailRepository)
        {
            this.emailService = emailService;
            this.iAMClient = iAMClient;
            this.emailRepository = emailRepository;
        }

        #region Methods
        public List<EmailMessageDTO> GetEmailMessages()
        {
            return emailRepository.GetAllEmails();
        }

        public async Task PublishEmail(EmailMessageDTO message)
        {
            emailRepository.AddEmail(message);

            await emailService.SendEmailAsync(message);
        }

        public async Task PublishEmailByIdentityNumber(IdentityNumberMessageDTO message)
        {
            var userEmail = await iAMClient.GetUserEmail(message.ToIdentityNumber);
            if (userEmail == null)
            {
                Console.WriteLine($"Identity number of user: {message.ToIdentityNumber} was ignored");
                return;
            }

            var emailMessage = new EmailMessageDTO()
            {
                BodyHtml = message.BodyHtml,
                IsHtml = message.IsHtml,
                Subject = message.Subject,
                ToEmail = userEmail
            };

            emailRepository.AddEmail(emailMessage);

            await emailService.SendEmailAsync(emailMessage);
        }
        #endregion
    }
}
