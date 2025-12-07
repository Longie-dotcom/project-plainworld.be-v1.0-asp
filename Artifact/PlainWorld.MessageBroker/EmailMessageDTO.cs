namespace PlainWorld.MessageBroker
{
    public class EmailMessageDTO
    {
        public string ToEmail { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string BodyHtml { get; set; } = string.Empty;
        public bool IsHtml { get; set; } = true;
    }

    public class UserIDMessageDTO
    {
        public Guid ToUserID { get; set; }
        public string Subject { get; set; } = string.Empty;
        public string BodyHtml { get; set; } = string.Empty;
        public bool IsHtml { get; set; } = true;
    }
}
