using Application.DTO;

namespace Application.Interface.IService
{
    public interface IUserService
    {
        Task<IEnumerable<UserDTO>> GetUsersAsync(
            string? sortBy, 
            QueryUserDTO dto, 
            string createdBy,
            string role);
        Task<UserDetailDTO?> GetUserByIdAsync(
            Guid userId, 
            string createdBy,
            string role);

        Task<UserDTO> CreateUserAsync(
            UserCreateDTO dto, 
            string createdBy,
            string role);
        Task<UserDTO> UpdateUserAsync(
            Guid userId, 
            UserUpdateDTO dto, 
            string createdBy,
            string role);
        Task DeleteUserAsync(
            Guid userId,
            UserDeleteDTO dto, 
            string createdBy,
            string role);
        Task ChangePasswordAsync(
            string identityNumber,
            ChangePasswordDTO dto);

        Task PatientSyncUpdating(IAMConsumeUpdateDTO dto);
        Task PatientSyncDeleting(IAMConsumeDeleteDTO dto);
    }
}