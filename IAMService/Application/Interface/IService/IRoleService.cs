using Application.DTO;

namespace Application.Interface.IService
{
    public interface IRoleService
    {
        Task<RoleDetailDTO> GetRoleByIdAsync(
            Guid roleId);
        Task<IEnumerable<RoleDTO>> GetRoleListAsync();

        Task CreateRoleAsync(
            RoleCreateDTO dto, 
            Guid createdBy);
        Task UpdateRoleAsync(
            Guid roleId, 
            RoleUpdateDTO dto, 
            Guid createdBy);
        Task DeleteRoleAsync(
            Guid roleId, 
            Guid createdBy);
    }
}
