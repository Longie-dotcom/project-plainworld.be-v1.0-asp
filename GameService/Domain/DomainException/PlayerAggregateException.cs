namespace Domain.DomainException
{
    public class PlayerAggregateException : Exception
    {
        public PlayerAggregateException(string message) : base(message) { }
    }
}
