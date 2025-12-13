using Application.DTO;

namespace Application.Interface.IService
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDTO>> GetRoleListAsync(
            QueryRoleDTO dto);
        Task<RoleDetailDTO> GetRoleByIdAsync(
            Guid roleId);

        Task CreateRoleAsync(
            RoleCreateDTO dto, 
            Guid createdBy);
        Task UpdateRoleInfoAsync(
            Guid roleId, 
            RoleUpdateDTO dto, 
            Guid createdBy);

        Task UpdateRolePrivilegeAsync(
            Guid roleId,
            RolePrivilegeUpdateDTO dto,
            Guid createdBy);

        Task DeleteRoleAsync(
            Guid roleId, 
            Guid createdBy);
    }
}
