using Application.ApplicationException;
using Application.DTO;
using Application.Enum;
using Application.Interface.IPublisher;
using Application.Interface.IService;
using AutoMapper;
using Domain.Aggregate;
using Domain.IRepository;
using PlainWorld.MessageBroker;

namespace Application.Service
{
    public class UserService : IUserService
    {
        #region Attributes
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IUserUpdatePublisher iAMUpdatePublisher;
        private readonly IUserDeletePublisher iAMDeletePublisher;
        #endregion

        #region Properties
        #endregion

        public UserService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IUserDeletePublisher iAMDeletePublisher,
            IUserUpdatePublisher iAMUpdatePublisher)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.iAMDeletePublisher = iAMDeletePublisher;
            this.iAMUpdatePublisher = iAMUpdatePublisher;
        }

        #region Methods
        public async Task<IEnumerable<UserDTO>> GetUsersAsync(
            string? sortBy,
            QueryUserDTO dto,
            Guid createdBy,
            string role)
        {
            var users = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetUsersWithFilterAsync(
                    dto.PageIndex,
                    dto.PageLength,
                    dto.Search,
                    dto.Gender,
                    dto.IsActive,
                    dto.DateOfBirthFrom,
                    dto.DateOfBirthTo,
                    createdBy,
                    role,
                    sortBy);

            // Validate user accounts existence
            if (users == null || !users.Any())
                throw new UserNotFound(
                    "User account list is empty");

            // Map to DTO
            return mapper.Map<IEnumerable<UserDTO>>(users);
        }

        public async Task<UserDetailDTO> GetUserByIdAsync(
            Guid userId,
            Guid createdBy,
            string role)
        {
            var u = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByUserIdAsync(userId);

            // Validate user account existence and its owner
            var user = ValidateAccountModifying(createdBy, u, role);

            // Only Admin and Super Admin can see all account
            return mapper.Map<UserDetailDTO>(user);
        }

        public async Task CreateUserAsync(
            UserCreateDTO dto,
            Guid createdBy,
            string role)
        {
            // Validate email
            var existingByEmail = await unitOfWork
                .GetRepository<IUserRepository>()
                .ExistsByEmailAsync(dto.Email);
            if (existingByEmail)
                throw new UserAlreadyExists(
                    "Email already registered.");

            // Validate role
            if (dto.RoleCodes == null || dto.RoleCodes.Count == 0)
                throw new RoleNotFound(
                    "User must have at least one role.");

            // Apply domain
            var user = new User(
                userID: Guid.NewGuid(),
                email: dto.Email,
                fullName: dto.FullName,
                password: dto.Password,
                dob: dto.DateOfBirth,
                gender: dto.Gender,
                createdBy: createdBy,
                isActive: true
            );

            // Validate permited set role and add role
            foreach (var code in dto.RoleCodes)
            {
                var r = await unitOfWork
                .GetRepository<IRoleRepository>()
                    .GetByCodeAsync(code);

                if (r == null)
                    throw new RoleNotFound(
                        $"Role code '{code}' does not exist.");

                ValidateRoleModifying(role, r);

                user.AddRole(r.RoleID);
            }

            // Apply persistence
            await unitOfWork.BeginTransactionAsync();
            unitOfWork
                .GetRepository<IUserRepository>()
                .Add(user);
            await unitOfWork.CommitAsync(createdBy.ToString());
        }

        public async Task UpdateUserAsync(
            Guid userId,
            UserUpdateDTO dto,
            Guid createdBy,
            string role)
        {
            var u = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByUserIdAsync(userId);

            // Validate user account existence and its owner
            var user = ValidateAccountModifying(createdBy, u, role);

            // Apply domain
            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.UpdateFullName(dto.FullName);

            if (!string.IsNullOrWhiteSpace(dto.Gender))
                user.UpdateGender(dto.Gender);

            if (dto.DateOfBirth != null)
                user.UpdateDob(dto.DateOfBirth.Value);

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                // Check if another user contains the email (excluding current user)
                var existingByEmail = await unitOfWork
                    .GetRepository<IUserRepository>()
                    .GetAllAsync();
                if (existingByEmail.Any(u => u.UserID != userId && u.Email.Contains(
                    dto.Email, StringComparison.OrdinalIgnoreCase)))
                    throw new UserAlreadyExists("Email already registered.");
                user.UpdateEmail(dto.Email);
            }

            // Validate permitted update privilege
            var privilegeUpdates = dto.UserPrivilegeUpdateDTOs?
                .Select(p => (p.IsGranted, p.PrivilegeID))
                .ToList() ?? new List<(bool IsGranted, Guid PrivilegeID)>();
            ValidatePermittedPrivilege(user, privilegeUpdates, role);

            // Validate permited update role
            if (dto.UserRoleUpdateDTOs?.Any() == true)
            {
                foreach (var rId in dto.UserRoleUpdateDTOs)
                {
                    var r = await unitOfWork.GetRepository<IRoleRepository>()
                        .GetByIdAsync(rId.RoleID);

                    if (r == null)
                        throw new RoleNotFound(
                            $"Role with id: '{rId}' does not exist.");

                    ValidateRoleModifying(role, r);
                }
            }

            // Apply persistence
            await unitOfWork.BeginTransactionAsync();
            // Update user privileges
            await unitOfWork
                .GetRepository<IUserRepository>()
                .UpdateUserPrivilegesAsync(userId, privilegeUpdates);
            // Update user roles
            if (dto.UserRoleUpdateDTOs?.Any() == true)
            {
                var roleUpdates = dto.UserRoleUpdateDTOs
                    .Select(r => (r.IsActive, r.RoleID))
                    .ToList();

                await unitOfWork
                    .GetRepository<IUserRepository>()
                    .UpdateUserRolesAsync(userId, roleUpdates);
            }
            // Update user
            unitOfWork
                .GetRepository<IUserRepository>()
                .Update(user.UserID, user);
            await unitOfWork.CommitAsync(createdBy.ToString());

            // Publish message
            await iAMUpdatePublisher.PublishAsync(new UserUpdateRequestDTO()
            {
                UserID = user.UserID,
                Dob = dto.DateOfBirth,
                FullName = dto.FullName,
                Gender = dto.Gender,
                Email = dto.Email,
                IsActive = user.IsActive,
            });
        }

        public async Task DeleteUserAsync(
            Guid userId,
            Guid performedBy,
            Guid createdBy,
            string role)
        {
            var u = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByIdAsync(userId);

            // Validate user account existence and its owner
            var user = ValidateAccountModifying(createdBy, u, role);

            // Apply domain
            user.UpdateActive(false);

            // Apply persistence
            await unitOfWork.BeginTransactionAsync();
            unitOfWork
                .GetRepository<IUserRepository>()
                .Update(userId, user);
            await unitOfWork.CommitAsync(performedBy.ToString());

            // Publish message
            await iAMDeletePublisher.PublishAsync(new UserUpdateRequestDTO()
            {
                UserID = user.UserID,
                IsActive = false
            });
        }

        public async Task ChangePasswordAsync(
            Guid userId, 
            ChangePasswordDTO dto)
        {
            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByUserIdAsync(userId);

            // Validate user account existence
            if (user == null)
                throw new UserNotFound(
                    $"User with user ID: {userId} is not found");

            // Apply domain
            if (dto.NewPassword != dto.NewConfirmedPassword)
                throw new InvalidChangePassword(
                    "New password and confirmed password are not matched.");

            if (!user.Password.Verify(dto.OldPassword))
                throw new InvalidChangePassword(
                    "Old password does not matched.");

            user.ChangePassword(dto.NewPassword);

            // Apply persistence
            await unitOfWork.BeginTransactionAsync();
            unitOfWork
                .GetRepository<IUserRepository>()
                .Update(user.UserID, user);
            await unitOfWork.CommitAsync(userId.ToString());
        }

        public async Task UserSyncUpdating(UserUpdateRequestDTO dto)
        {
            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByUserIdAsync(dto.UserID);

            // Validate user account existence
            if (user == null)
                throw new UserNotFound(
                    $"Sync-up data can not be executed due to user ID: {dto.UserID} is not found");

            // Apply domain
            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.UpdateFullName(dto.FullName);

            if (!string.IsNullOrWhiteSpace(dto.Gender))
                user.UpdateGender(dto.Gender);

            if (dto.Dob.HasValue)
                user.UpdateDob(dto.Dob.Value);

            if (!string.IsNullOrWhiteSpace(dto.Email))
            {
                // Check if another user contains the email (excluding current user)
                var existingByEmail = await unitOfWork
                    .GetRepository<IUserRepository>()
                    .GetAllAsync();
                if (existingByEmail.Any(u => u.UserID != dto.UserID && u.Email.Contains(
                    dto.Email, StringComparison.OrdinalIgnoreCase)))
                    throw new UserAlreadyExists("Email already registered.");
                user.UpdateEmail(dto.Email);
            }

            // Apply persistence
            await unitOfWork.BeginTransactionAsync();
            unitOfWork
                .GetRepository<IUserRepository>()
                .Update(user.UserID, user);
            await unitOfWork.CommitAsync("Other services");
        }

        public async Task<UserDetailDTO?> GetUserByIdGrpcAsync(
            Guid userId)
        {
            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByUserIdAsync(userId);

            // Validate user account existence (Allow all to access this user if it existed)
            if (user == null)
                throw new UserNotFound(
                    $"Sync-up data can not be executed due to user ID: {userId} is not found");

            return mapper.Map<UserDetailDTO>(user);
        }
        #endregion

        #region Private Helpers
        private void ValidateRoleModifying(string userRole, Role role)
        {
            // Make sure admin can not create account that has higher nor equal permission as ADMIN or SUPER_ADMIN
            if (userRole == RoleKey.ADMIN && (
                role.Code == RoleKey.ADMIN ||
                role.Code == RoleKey.SUPER_ADMIN))
                throw new InvalidOwner(
                    $"User has no permission not set nor update an account to {role.Name}");

            // Super admin can set any role
        }

        private User ValidateAccountModifying(Guid createdBy, User? user, string role)
        {
            if (user == null)
                throw new UserNotFound(
                    $"User with user ID is not found with given ID");

            if (role == RoleKey.SUPER_ADMIN)
                return user;

            if (user.CreatedBy != createdBy)
                throw new InvalidOwner(
                    "Invalid account owner, user did not created this user account");

            return user;
        }

        private void ValidatePermittedPrivilege(
            User user,
            List<(bool IsGranted, Guid PrivilegeID)> privilegeUpdates,
            string userRole)
        {
            if (userRole == RoleKey.SUPER_ADMIN) return;

            bool isChanged = false;
            var oldPrivileges = user.UserPrivileges.ToDictionary(up => up.PrivilegeID, up => up.IsGranted);
            var newPrivileges = privilegeUpdates.ToDictionary(pu => pu.PrivilegeID, pu => pu.IsGranted);
            if (oldPrivileges.Count != newPrivileges.Count)
            {
                isChanged = true;
            }
            else
            {
                // Check if any privilege has different IsGranted
                isChanged = newPrivileges.Any(np =>
                    !oldPrivileges.TryGetValue(np.Key, out var oldGranted) || oldGranted != np.Value);
            }

            if (isChanged)
                throw new InvalidOwner(
                    "User can not grant nor revoke any privilege for this account");
        }
        #endregion
    }
}
