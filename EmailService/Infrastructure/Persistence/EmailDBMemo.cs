using FSA.LaboratoryManagement.EmailMessage;

namespace Infrastructure.Persistence
{
    public static class EmailDBMemo
    {
        public static List<EmailMessageDTO> EmailMessages { get; set; } = new();
    }
}
