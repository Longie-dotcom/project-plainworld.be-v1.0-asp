using Application.Interface;
using FSA.LaboratoryManagement.EmailMessage;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.Service
{
    public class EmailService : IEmailService
    {
        private readonly string fromEmail;
        private readonly string displayName;
        private readonly string smtpHost;
        private readonly int smtpPort;
        private readonly string username;
        private readonly string password;
        private readonly bool enableSsl;

        public EmailService(
            string fromEmail,
            string displayName,
            string smtpHost,
            int smtpPort,
            string username,
            string password,
            bool enableSsl)
        {
            this.fromEmail = fromEmail;
            this.displayName = displayName;
            this.smtpHost = smtpHost;
            this.smtpPort = smtpPort;
            this.username = username;
            this.password = password;
            this.enableSsl = enableSsl;
        }

        public async Task SendEmailAsync(EmailMessageDTO email)
        {
            var message = new MailMessage
            {
                From = new MailAddress(fromEmail, displayName),
                Subject = email.Subject,
                Body = email.BodyHtml,
                IsBodyHtml = email.IsHtml
            };

            message.To.Add(email.ToEmail);

            using var smtp = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl = enableSsl
            };

            await smtp.SendMailAsync(message);
        }
    }
}
