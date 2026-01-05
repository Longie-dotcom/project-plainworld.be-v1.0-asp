namespace Infrastructure.InfrastructureException
{
    public class TemporaryException : Exception
    {
        public TemporaryException(string message) : base(message) { }
    }
}
