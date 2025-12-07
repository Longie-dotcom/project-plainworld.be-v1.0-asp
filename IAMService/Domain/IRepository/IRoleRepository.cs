using Domain.Aggregate;
namespace Domain.IRepository
{
    public interface IRoleRepository : 
        IGenericRepository<Role>, 
        IRepositoryBase
    {
        Task<IEnumerable<Role>> GetRolesAsync();
        Task<Role> GetByDetailByIdAsync(Guid roleId);
        Task<Role> GetByCodeAsync(string code);

        Task UpdateRolePrivilegesAsync(Guid roleId, IEnumerable<Guid> newPrivilegeIds);
    }
}
