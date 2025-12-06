using Application.Enum;
using Domain.IRepository;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IAMServer.InstrumentClient.gRPC;
using Microsoft.AspNetCore.Authorization;

namespace Application.GrpcService
{
    [AllowAnonymous]
    public class InstrumentGrpcService : IAMGrpc.IAMGrpcBase
    {
        private readonly IUnitOfWork unitOfWork;

        public InstrumentGrpcService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public override async Task<GetLabUserResponse> GetLabUser(
            GetLabUserRequest request, ServerCallContext context)
        {
            var labUser = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByIdentityNumberAsync(request.IdentityNumber);

            if (labUser == null)
                throw new RpcException(
                    new Status(StatusCode.NotFound, "Lab user not found"));

            var islabUser = labUser.UserRoles
                .Any(x => x.Role.Code == RoleKey.LAB_USER);

            if (!islabUser)
                throw new RpcException(
                    new Status(StatusCode.PermissionDenied, "The given user is not a lab user"));

            return new GetLabUserResponse
            {
                FullName = labUser.FullName,
                Address = labUser.Address,
                Gender = labUser.Gender,
                IdentityNumber = labUser.IdentityNumber,
                IsActive = labUser.IsActive,
                Dob = Timestamp.FromDateTime(labUser.Dob.ToUniversalTime()),
                Email = labUser.Email,
                Phone = labUser.Phone,
            };
        }
    }
}
