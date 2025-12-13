using Application.ApplicationException;
using Application.DTO;
using Application.Helper;
using Application.Interface.IService;
using AutoMapper;
using Domain.Aggregate;
using Domain.IRepository;
using System.Data;

namespace Infrastructure.Service
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public RoleService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        #region Methods
        public async Task<IEnumerable<RoleDTO>> GetRoleListAsync(QueryRoleDTO dto)
        {
            var roles = await unitOfWork
                .GetRepository<IRoleRepository>()
                .GetRolesWithFilterAsync(
                    dto.PageIndex, 
                    dto.PageLength, 
                    dto.Search);

            // Validate role existence
            if (roles == null || !roles.Any())
                throw new RoleNotFound(
                    $"Role list is empty");

            return mapper.Map<IEnumerable<RoleDTO>>(roles);
        }

        public async Task<RoleDetailDTO> GetRoleByIdAsync(Guid roleId)
        {
            var role = await unitOfWork
                .GetRepository<IRoleRepository>()
                .GetByDetailByIdAsync(roleId);

            // Validate role existence
            if (role == null)
                throw new RoleNotFound(
                    $"Role with role ID: {roleId} is not found");

            return mapper.Map<RoleDetailDTO>(role);
        }

        public async Task CreateRoleAsync(
            RoleCreateDTO dto,
            Guid createdBy)
        {
            var existedRoles = await unitOfWork
                .GetRepository<IRoleRepository>()
                .GetAllAsync();

            // Validate role code duplication
            var existedCode = existedRoles.Where(r => r.Code == dto.Code);
            if (existedCode.Any())
                throw new RoleCodeAlreadyExists(
                    $"Role with code '{dto.Code}' already exists.");
            
            // Apply domain
            var newRole = new Role
            (
                Guid.NewGuid(),
                dto.Name,
                dto.Code,
                dto.Description
            );

            // Link privileges
            foreach (var privilegeId in dto.PrivilegeID)
            {
                newRole.AddPrivilege(privilegeId);
            }

            // Apply persistence
            await unitOfWork.BeginTransactionAsync();
            unitOfWork
                .GetRepository<IRoleRepository>()
                .Add(newRole);           
            await unitOfWork.CommitAsync(createdBy.ToString());
        }

        public async Task UpdateRoleInfoAsync(
            Guid roleId, 
            RoleUpdateDTO dto,
            Guid createdBy)
        {
            var existing = await unitOfWork
                .GetRepository<IRoleRepository>()
                .GetByIdAsync(roleId);

            // Validate role existence
            if (existing == null)
                throw new RoleNotFound(
                    $"Role with role ID: {roleId} is not found");

            // Apply domain
            existing.UpdateName(dto.Name);
            existing.UpdateDescription(dto.Description);

            // Apply persistence
            await unitOfWork.BeginTransactionAsync();
            unitOfWork
                .GetRepository<IRoleRepository>()
                .Update(roleId, existing);
            await unitOfWork.CommitAsync(createdBy.ToString());
        }

        public async Task UpdateRolePrivilegeAsync(
            Guid roleId,
            RolePrivilegeUpdateDTO dto,
            Guid createdBy)
        {
            // Apply persistence
            await unitOfWork.BeginTransactionAsync();
            await unitOfWork
                .GetRepository<IRoleRepository>()
                .UpdateRolePrivilegesAsync(roleId, dto.PrivilegeID);
            await unitOfWork.CommitAsync(createdBy.ToString());
        }

        public async Task DeleteRoleAsync(
            Guid roleId, 
            Guid createdBy)
        {
            var role = await unitOfWork
                .GetRepository<IRoleRepository>()
                .GetByIdAsync(roleId);

            // Validate role existence
            if (role == null)
                throw new RoleNotFound(
                    $"Role with role ID: {roleId} is not found");

            // Apply persistence
            await unitOfWork.BeginTransactionAsync();
            unitOfWork
                .GetRepository<IRoleRepository>()
                .Remove(roleId);
            await unitOfWork.CommitAsync(createdBy.ToString());
        }
        #endregion
    }
}
