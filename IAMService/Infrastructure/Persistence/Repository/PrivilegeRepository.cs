using Domain.Aggregate;
using Domain.IRepository;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repository
{
    public class PrivilegeRepository : 
        GenericRepository<Privilege>, 
        IPrivilegeRepository
    {
        #region Attributes
        #endregion

        #region Properties
        #endregion

        public PrivilegeRepository(IAMDBContext context) : base (context) { }

        #region Methods
        public async Task<IEnumerable<Privilege>> GetRolesWithFilterAsync(
            int? pageIndex,
            int? pageSize,
            string? search = null)
        {
            var query = context.Privileges.AsQueryable();

            // 1. Apply search/filter
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.Description.Contains(search) ||
                    u.Name.Contains(search));
            }

            // 2. TODO: Apply role-based visibility if needed

            // 3. Apply default sorting (by Name)
            query = query.OrderBy(u => u.Name);

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

        public async Task<bool> ExistsByNameAsync(string name)
        {
            return await context.Privileges
                .AnyAsync(p => p.Name == name);
        }

        public async Task<bool> ExistsByNameExceptIdAsync(string name, Guid privilegeId)
        {
            return await context.Privileges
                .AnyAsync(p => p.Name == name && p.PrivilegeID != privilegeId);
        }
        #endregion
    }
}
