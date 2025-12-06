using Domain.IRepository;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IAMServer.EmailClient.gRPC;
using Microsoft.AspNetCore.Authorization;

namespace Application.GrpcService
{
    [AllowAnonymous]
    public class EmailGrpcService : IAMGrpc.IAMGrpcBase
    {
        private readonly IUnitOfWork unitOfWork;

        public EmailGrpcService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public override async Task<EmailServiceGetUserResponse> EmailServiceGetUser(
            EmailServiceGetUserRequest request, ServerCallContext context)
        {
            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByIdentityNumberAsync(request.IdentityNumber);

            if (user == null)
                throw new RpcException(
                    new Status(StatusCode.NotFound, "Lab user not found"));

            return new EmailServiceGetUserResponse
            {
                FullName = user.FullName,
                Address = user.Address,
                Gender = user.Gender,
                IdentityNumber = user.IdentityNumber,
                IsActive = user.IsActive,
                Dob = Timestamp.FromDateTime(user.Dob.ToUniversalTime()),
                Email = user.Email,
                Phone = user.Phone,
            };
        }
    }
}
