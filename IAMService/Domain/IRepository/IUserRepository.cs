using Domain.Aggregate;
namespace Domain.IRepository
{
    public interface IUserRepository : 
        IGenericRepository<User>, 
        IRepositoryBase
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByPhoneAsync(string phone);
        Task<User?> GetByIdentityNumberAsync(string identityNumber);

        Task<bool> ExistsByEmailAsync(string email, Guid? excludeUserId = null);
        Task<bool> ExistsByPhoneAsync(string phone, Guid? excludeUserId = null);
        Task<bool> ExistsByIdentityNumberAsync(string identityNumber, Guid? excludeUserId = null);

        Task UpdateUserRolesAsync(Guid userId, List<(bool IsActive, Guid RoleId)> roles);
        Task UpdateUserPrivilegesAsync(Guid userId, List<(bool IsGranted, Guid PrivilegeId)> privileges);

        Task<User?> GetByDetailByIdAsync(Guid userId);
        Task<IEnumerable<User>> GetUsersWithFilterAsync(
            int pageIndex,
            int pageSize,
            string? search = null,
            string? gender = null,
            bool? isActive = null,
            DateTime? dateOfBirthFrom = null,
            DateTime? dateOfBirthTo = null,
            string? createdBy = null,
            string? role = null,
            string? sortBy = null);
    }
}
