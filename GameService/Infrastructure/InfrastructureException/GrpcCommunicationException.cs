namespace Infrastructure.InfrastructureException
{
    public class GrpcCommunicationException : Exception
    {
        public GrpcCommunicationException(string message) : base(message) { }
    }
}
