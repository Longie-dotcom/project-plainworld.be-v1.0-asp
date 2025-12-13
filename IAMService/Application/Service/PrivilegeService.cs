using Application.ApplicationException;
using Application.DTO;
using Application.Interface.IService;
using AutoMapper;
using Domain.Aggregate;
using Domain.IRepository;

namespace Infrastructure.Service
{
    public class PrivilegeService : IPrivilegeService
    {
        #region Attributes
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        #endregion

        public PrivilegeService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        #region Methods
        public async Task<IEnumerable<PrivilegeDTO>> GetPrivilegesAsync(QueryPrivilegeDTO dto)
        {
            var privileges = await unitOfWork
                .GetRepository<IPrivilegeRepository>()
                .GetRolesWithFilterAsync(
                    dto.PageIndex,
                    dto.PageLength,
                    dto.Search);

            // Validate privilege existence
            if (privileges == null || !privileges.Any())
                throw new PrivilegeNotFound(
                    "Privileges list is empty.");

            return mapper.Map<IEnumerable<PrivilegeDTO>>(privileges);
        }

        public async Task<PrivilegeDTO> GetPrivilegeByIdAsync(Guid privilegeId)
        {
            var privilege = await unitOfWork
                .GetRepository<IPrivilegeRepository>()
                .GetByIdAsync(privilegeId);

            // Validate privilege existence
            if (privilege == null)
                throw new PrivilegeNotFound(
                    $"Privilege with privilege ID: {privilegeId} is not found");

            return mapper.Map<PrivilegeDTO>(privilege);
        }

        public async Task CreatePrivilegeAsync(
            PrivilegeCreateDTO dto,
            Guid createdBy)
        {
            // Validate existed code
            var existedCode = await unitOfWork
                .GetRepository<IPrivilegeRepository>()
                .ExistsByNameAsync(dto.Name);
            if (existedCode)
                throw new PrivilegeAlreadyExists(
                    $"Privilege name '{dto.Name}' already exists.");

            // Apply domain
            var privilege = new Privilege(Guid.NewGuid(), dto.Name, dto.Description);

            // Apply persistence
            await unitOfWork.BeginTransactionAsync();
            unitOfWork
                .GetRepository<IPrivilegeRepository>()
                .Add(privilege);
            await unitOfWork.CommitAsync(createdBy.ToString());
        }

        public async Task UpdatePrivilegeAsync(
            Guid privilegeId, 
            PrivilegeUpdateDTO dto,
            Guid createdBy)
        {
            var privilege = await unitOfWork
                .GetRepository<IPrivilegeRepository>()
                .GetByIdAsync(privilegeId);

            // Validate privilege existence
            if (privilege == null)
                throw new PrivilegeNotFound(
                    $"Privilege with ID '{privilegeId}' not found.");

            // Validate existed name
            var existedName = await unitOfWork
                .GetRepository<IPrivilegeRepository>()
                .ExistsByNameExceptIdAsync(dto.Name, privilegeId);
            if (existedName)
                throw new PrivilegeAlreadyExists(
                    $"Privilege name '{dto.Name}' already exists.");

            // Apply domain
            privilege.UpdateNameAndDescription(dto.Name, dto.Description);

            // Apply persistence
            await unitOfWork.BeginTransactionAsync();
            unitOfWork
                .GetRepository<IPrivilegeRepository>()
                .Update(privilegeId, privilege);
            await unitOfWork.CommitAsync(createdBy.ToString());
        }

        public async Task DeletePrivilegeAsync(
            Guid privilegeId,
            Guid createdBy)
        {
            var privilege = await unitOfWork
                .GetRepository<IPrivilegeRepository>()
                .GetByIdAsync(privilegeId);

            // Validate privilege existence
            if (privilege == null)
                throw new PrivilegeNotFound(
                    $"Privilege with ID '{privilegeId}' not found.");

            // Apply persistence
            await unitOfWork.BeginTransactionAsync();
            unitOfWork
                .GetRepository<IPrivilegeRepository>()
                .Remove(privilegeId);
            await unitOfWork.CommitAsync(createdBy.ToString());
        }
        #endregion
    }
}
