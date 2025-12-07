using Application.Enum;
using Domain.Aggregate;
using Domain.Entity;
using Domain.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repository
{
    public class UserRepository :
        GenericRepository<User>,
        IUserRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public UserRepository(IAMDBContext context) : base(context) { }

        #region Methods
        public async Task<IEnumerable<User>> GetUsersWithFilterAsync(
            int pageIndex,
            int pageSize,
            string? search = null,
            string? gender = null,
            bool? isActive = null,
            DateTime? dateOfBirthFrom = null,
            DateTime? dateOfBirthTo = null,
            Guid? createdBy = null,
            string? role = null,
            string? sortBy = null)
        {
            var query = context.Users.AsQueryable();

            // 1. Apply search/filter first
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.FullName.Contains(search) ||
                    u.Email.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(gender))
                query = query.Where(u => u.Gender == gender);

            if (isActive.HasValue)
                query = query.Where(u => u.IsActive == isActive.Value);

            // 2. Apply role-based visibility


            // 3. Apply sorting dynamically
            query = sortBy switch
            {
                SortKeyword.SORT_BY_EMAIL => query.OrderBy(u => u.Email),
                SortKeyword.SORT_BY_AGE => query.OrderBy(u => u.Dob), // ascending age
                SortKeyword.SORT_BY_GENDER => query.OrderBy(u => u.Gender),
                _ => query.OrderBy(u => u.FullName)
            };

            // 4. Pagination
            if (pageIndex <= 0) pageIndex = 1;
            var skip = (pageIndex - 1) * pageSize;

            return await query
                .Skip(skip)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<User?> GetByUserIdAsync(Guid userId)
        {
            return await context.Users
                .AsNoTracking()
                .Include(u => u.UserPrivileges)
                    .ThenInclude(up => up.Privilege)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePrivileges)
                            .ThenInclude(rp => rp.Privilege)
                .Include(u => u.RefreshToken)
                .FirstOrDefaultAsync(u => u.UserID == userId);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await context.Users
                .AsNoTracking()
                .Include(u => u.UserPrivileges)
                    .ThenInclude(up => up.Privilege)
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePrivileges)
                            .ThenInclude(rp => rp.Privilege)
                .Include(u => u.RefreshToken)
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<bool> ExistsByEmailAsync(string email, Guid? excludeUserId = null)
        {
            var query = context.Users.AsQueryable();

            if (excludeUserId.HasValue)
                query = query.Where(u => u.UserID != excludeUserId.Value);

            return await query.AnyAsync(u => u.Email == email);
        }

        public async Task UpdateUserRolesAsync(
            Guid userId, List<(bool IsActive, Guid RoleId)> roles)
        {
            var userRoles = await context.UserRoles
                .Where(ur => ur.UserID == userId)
                .ToListAsync();

            foreach (var (isActive, roleId) in roles)
            {
                var existing = userRoles.FirstOrDefault(ur => ur.RoleID == roleId);

                if (existing == null && isActive)
                {
                    // Add new active role
                    context.UserRoles.Add(new UserRole
                    (
                        Guid.NewGuid(),
                        roleId,
                        userId,
                        true
                    ));
                }
                else if (existing != null)
                {
                    // Update active/inactive status
                    if (isActive)
                    {
                        existing.Activate();
                    } else
                    {
                        existing.Deactivate();
                    }
                    context.UserRoles.Update(existing);
                }
            }
        }

        public async Task UpdateUserPrivilegesAsync(
            Guid userId,
            List<(bool IsGranted, Guid PrivilegeId)> privileges)
        {
            // Remove all current privileges for the user
            var currentPrivileges = await context.UserPrivileges
                .Where(up => up.UserID == userId)
                .ToListAsync();

            context.UserPrivileges.RemoveRange(currentPrivileges);

            // Add all new privileges
            var newPrivileges = privileges.Select(p => new UserPrivilege(
                Guid.NewGuid(),
                p.PrivilegeId,
                userId,
                p.IsGranted
            ));

            await context.UserPrivileges.AddRangeAsync(newPrivileges);
        }
        #endregion
    }
}
