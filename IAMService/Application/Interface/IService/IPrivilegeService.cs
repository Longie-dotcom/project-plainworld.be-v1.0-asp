using Application.DTO;

namespace Application.Interface.IService
{
    public interface IPrivilegeService
    {
        Task<PrivilegeDTO> GetPrivilegeByIdAsync(
            Guid privilegeId);
        Task<IEnumerable<PrivilegeDTO>> GetPrivilegesAsync();

        Task CreatePrivilegeAsync(
            PrivilegeCreateDTO dto,
            Guid createdBy);
        Task UpdatePrivilegeAsync(
            Guid PrivilegeId, 
            PrivilegeUpdateDTO dto,
            Guid createdBy);
        Task DeletePrivilegeAsync(
            Guid PrivilegeId,
            Guid createdBy);
    }
}
