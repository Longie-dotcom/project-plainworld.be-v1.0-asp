using Application.DTO;
using PlainWorld.MessageBroker;

namespace Application.Interface.IService
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetUsersAsync(
            string? sortBy, 
            QueryUserDTO dto,
            Guid createdBy,
            string role);
        Task<UserDetailDTO> GetUserByIdAsync(
            Guid userId,
            Guid createdBy,
            string role);

        Task CreateUserAsync(
            UserCreateDTO dto,
            Guid createdBy,
            string role);
        Task UpdateUserAsync(
            Guid userId, 
            UserUpdateDTO dto,
            Guid createdBy,
            string role);
        Task DeleteUserAsync(
            Guid userId,
            Guid performedBy,
            Guid createdBy,
            string role);
        Task ChangePasswordAsync(
            Guid userId,
            ChangePasswordDTO dto);

        Task UserSyncUpdating(
            UserUpdateRequestDTO dto);
        Task<UserDetailDTO?> GetUserByIdGrpcAsync(
            Guid userId);
    }
}