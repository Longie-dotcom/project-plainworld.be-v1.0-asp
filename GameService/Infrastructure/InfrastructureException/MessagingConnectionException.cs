namespace Infrastructure.InfrastructureException
{
    public class MessagingConnectionException : Exception
    {
        public MessagingConnectionException(string message) : base(message) { }
    }
}
