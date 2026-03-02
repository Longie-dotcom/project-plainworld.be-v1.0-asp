namespace Domain.DomainException
{
    public class ChatAggregateException : Exception
    {
        public ChatAggregateException(string message) : base(message) { }
    }
}
