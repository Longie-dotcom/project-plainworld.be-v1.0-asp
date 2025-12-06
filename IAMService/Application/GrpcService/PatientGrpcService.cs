using Domain.IRepository;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.AspNetCore.Authorization;
using IAMServer.PatientClient.gRPC;
using Application.Enum;

namespace Application.GrpcService
{
    [AllowAnonymous]
    public class PatientGrpcService : IAMGrpc.IAMGrpcBase
    {
        private readonly IUnitOfWork unitOfWork;

        public PatientGrpcService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public override async Task<GetUserResponse> GetUser(GetUserRequest request, ServerCallContext context)
        {
            var patient = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByIdentityNumberAsync(request.IdentityNumber);

            if (patient == null)
                throw new RpcException(
                    new Status(StatusCode.NotFound, "Patient not found"));

            var isPatient = patient.UserRoles
                .Any(x => x.Role.Code == RoleKey.NORMAL_USER);

            if (!isPatient)
                throw new RpcException(
                    new Status(StatusCode.PermissionDenied, "The given user is not a patient"));

            return new GetUserResponse
            {
                FullName = patient.FullName,
                Address = patient.Address,
                Gender = patient.Gender,
                IdentityNumber = patient.IdentityNumber,
                IsActive = patient.IsActive,
                Dob = Timestamp.FromDateTime(patient.Dob.ToUniversalTime()),
                Email = patient.Email,
                Phone = patient.Phone,
            };
        }
    }
}
