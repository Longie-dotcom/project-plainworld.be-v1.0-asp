namespace API.APIException
{
    public class ClaimNotFound : Exception
    {
        public ClaimNotFound(string message) : base(message) { }
    }
}
