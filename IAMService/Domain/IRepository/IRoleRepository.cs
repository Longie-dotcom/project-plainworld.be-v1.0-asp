using Domain.Aggregate;
namespace Domain.IRepository
{
    public interface IRoleRepository : 
        IGenericRepository<Role>, 
        IRepositoryBase
    {
        Task<IEnumerable<Role>> GetRolesWithFilterAsync(
            int? pageIndex,
            int? pageSize,
            string? search = null);
        Task<Role?> GetByDetailByIdAsync(Guid roleId);
        Task<Role?> GetByCodeAsync(string code);

        Task UpdateRolePrivilegesAsync(Guid roleId, IEnumerable<Guid> newPrivilegeIds);
    }
}
