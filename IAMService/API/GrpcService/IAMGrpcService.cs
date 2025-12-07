using Application.ApplicationException;
using Application.Interface.IService;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IAMServer.gRPC;

namespace API.GrpcService
{
    public class IAMGrpcService : IAMGrpc.IAMGrpcBase
    {
        #region Attributes
        private readonly IUserService userService;
        #endregion

        #region Properties
        #endregion

        public IAMGrpcService(
            IUserService userService)
        {
            this.userService = userService;
        }

        #region Methods
        public override async Task<GetUserResponse> GetUser(
            GetUserRequest request, 
            ServerCallContext context)
        {
            // Convert string user_id to Guid
            Guid userId;
            if (!Guid.TryParse(request.UserId, out userId))
            {
                throw new RpcException(
                    new Status(StatusCode.InvalidArgument, "Invalid user_id format"));
            }

            var user = await userService.GetUserByIdGrpcAsync(userId);
            if (user == null)
                throw new UserNotFound(
                    $"User with user ID: {userId} is not found");

            var response = new GetUserResponse
            {
                Email = user.Email,
                FullName = user.FullName,
                Gender = user.Gender,
                IsActive = true,
                Dob = Timestamp.FromDateTime(user.Dob)
            };

            return response;
        }
        #endregion
    }
}
