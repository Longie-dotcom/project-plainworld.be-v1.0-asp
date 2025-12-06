namespace API.APIException
{
    public class ClaimNotFound : Exception
    {
        public ClaimNotFound(string roleCode)
            : base($"Role with code '{roleCode}' was not found.") { }
    }
}
