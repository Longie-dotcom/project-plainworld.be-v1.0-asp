using IAMServer.EmailClient.gRPC;

namespace Application.Interface
{
    public interface IIAMClient
    {
        Task<string> GetUserEmail(string identityNumber);
    }
}
