namespace Application.Interface
{
    public interface IIAMClient
    {
        Task<string> GetUserEmail(Guid userId);
    }
}
