using Domain.Aggregate;
namespace Domain.IRepository
{
    public interface IPrivilegeRepository : 
        IGenericRepository<Privilege>,
        IRepositoryBase
    {
        Task<IEnumerable<Privilege>> GetRolesWithFilterAsync(
            int? pageIndex,
            int? pageSize,
            string? search = null);

        Task<bool> ExistsByNameAsync(string name);
        Task<bool> ExistsByNameExceptIdAsync(string name, Guid privilegeId);
    }
}
