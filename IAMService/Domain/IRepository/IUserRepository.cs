using Domain.Aggregate;
namespace Domain.IRepository
{
    public interface IUserRepository : 
        IGenericRepository<User>, 
        IRepositoryBase
    {
        Task<IEnumerable<User>> GetUsersWithFilterAsync(
            int pageIndex,
            int pageSize,
            string? search = null,
            string? gender = null,
            bool? isActive = null,
            DateTime? dateOfBirthFrom = null,
            DateTime? dateOfBirthTo = null,
            Guid? createdBy = null,
            string? role = null,
            string? sortBy = null);
        Task<User?> GetByUserIdAsync(Guid userId);
        Task<User?> GetByEmailAsync(string email);
        Task<bool> ExistsByEmailAsync(string email, Guid? excludeUserId = null);

        Task UpdateUserRolesAsync(Guid userId, List<(bool IsActive, Guid RoleId)> roles);
        Task UpdateUserPrivilegesAsync(Guid userId, List<(bool IsGranted, Guid PrivilegeId)> privileges);
    }
}
