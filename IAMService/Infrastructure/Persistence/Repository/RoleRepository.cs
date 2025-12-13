using Domain.Aggregate;
using Domain.Entity;
using Domain.IRepository;
using Infrastructure.InfrastructureException;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repository
{
    public class RoleRepository : 
        GenericRepository<Role>, 
        IRoleRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion
        public RoleRepository(IAMDBContext context) : base(context) { }

        #region Methods
        public async Task<IEnumerable<Role>> GetRolesWithFilterAsync(
            int? pageIndex,
            int? pageSize,
            string? search = null)
        {
            var query = context.Roles.AsQueryable();

            // 1. Apply search/filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.Code.Contains(search) ||
                    u.Name.Contains(search));
            }

            // 2. TODO: Apply role-based visibility if needed

            // 3. Apply default sorting (by Code)
            query = query.OrderBy(u => u.Code);

            // 4. Apply pagination if requested
            if (pageIndex.HasValue && pageSize.HasValue)
            {
                if (pageIndex <= 0) pageIndex = 1;
                if (pageSize <= 0) pageSize = 10;

                var skip = (pageIndex.Value - 1) * pageSize.Value;
                query = query.Skip(skip).Take(pageSize.Value);
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<Role?> GetByDetailByIdAsync(Guid roleId)
        {
            var role = await context.Roles
                .AsNoTracking()
                .Include(r => r.RolePrivileges)
                    .ThenInclude(rq => rq.Privilege)
                .FirstOrDefaultAsync(r => r.RoleID == roleId);

            return role;
        }

        public async Task<Role?> GetByCodeAsync(string code)
        {
            var role = await context.Roles
                .AsNoTracking()
                .Include(r => r.RolePrivileges)
                    .ThenInclude(rq => rq.Privilege)
                .FirstOrDefaultAsync(r => r.Code == code);

            return role;
        }

        public async Task UpdateRolePrivilegesAsync(Guid roleId, IEnumerable<Guid> newPrivilegeIds)
        {
            var role = await context.Roles
                .Include(r => r.RolePrivileges) // load tracked RolePrivileges
                .FirstOrDefaultAsync(r => r.RoleID == roleId);

            if (role == null)
                throw new RepositoryException(
                    $"Role with role ID: {roleId} is not found");

            // Remove all existing privileges
            context.RolePrivileges.RemoveRange(role.RolePrivileges);

            // Add new privileges
            foreach (var pid in newPrivilegeIds.Distinct())
            {
                context.RolePrivileges.Add(new RolePrivilege(Guid.NewGuid(), role.RoleID, pid, true));
            }

            await context.SaveChangesAsync();
        }
        #endregion
    }
}
