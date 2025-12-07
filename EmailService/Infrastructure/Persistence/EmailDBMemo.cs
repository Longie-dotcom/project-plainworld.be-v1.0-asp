using PlainWorld.MessageBroker;

namespace Infrastructure.Persistence
{
    public static class EmailDBMemo
    {
        public static List<EmailMessageDTO> EmailMessages { get; set; } = new();
    }
}
