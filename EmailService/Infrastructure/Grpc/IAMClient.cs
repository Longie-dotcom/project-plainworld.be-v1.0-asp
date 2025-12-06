using Application.Interface;
using Grpc.Net.Client;
using IAMServer.EmailClient.gRPC;
using Infrastructure.InfrastructureException;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Grpc
{
    public class IAMClient : IIAMClient
    {
        private readonly IAMGrpc.IAMGrpcClient client;

        public IAMClient(IConfiguration configuration)
        {
            var grpcServerUrl = configuration["GRPC_IAM_SERVICE"];
            if (string.IsNullOrEmpty(grpcServerUrl))
                throw new GrpcCommunicationException("gRPC server URL is missing.");

            var channel = GrpcChannel.ForAddress(grpcServerUrl);
            client = new IAMGrpc.IAMGrpcClient(channel);
        }

        public async Task<string?> GetUserEmail(string identityNumber)
        {
            try
            {
                var user = await client.EmailServiceGetUserAsync(
                    new EmailServiceGetUserRequest()
                    {
                        IdentityNumber = identityNumber
                    });

                if (user == null)
                    return null;

                return user.Email;
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
