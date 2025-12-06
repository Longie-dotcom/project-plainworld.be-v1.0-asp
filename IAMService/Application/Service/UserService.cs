using Application.ApplicationException;
using Application.DTO;
using Application.Enum;
using Application.Interface.IPublisher;
using Application.Interface.IService;
using AutoMapper;
using Domain.Aggregate;
using Domain.DomainException;
using Domain.IRepository;

namespace Application.Service
{
    public class UserService : IUserService
    {
        #region Attributes
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IIAMUpdatePublisher iAMUpdatePublisher;
        private readonly IIAMDeletePublisher iAMDeletePublisher;
        #endregion

        #region Properties
        #endregion

        public UserService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IIAMDeletePublisher iAMDeletePublisher,
            IIAMUpdatePublisher iAMUpdatePublisher)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.iAMDeletePublisher = iAMDeletePublisher;
            this.iAMUpdatePublisher = iAMUpdatePublisher;
        }

        #region Methods
        public async Task<UserDetailDTO> GetUserByIdAsync(
            Guid userId,
            string createdBy,
            string role)
        {
            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByDetailByIdAsync(userId);

            if (user == null)
                throw new UserNotFound();

            // Only Admin and Super Admin can see all account
            if (user.CreatedBy != createdBy && role == RoleKey.LAB_MANAGER)
                throw new InvalidOwner(
                    "Invalid account owner, user did not created this user account");

            var dto = mapper.Map<UserDetailDTO>(user);

            // Get privileges of each role
            var roleDtos = new List<UserRoleDTO>();
            foreach (var userRole in user.UserRoles)
            {
                var roleDto = mapper.Map<RoleDetailDTO>(userRole.Role);
                roleDto.Privileges =
                    userRole.Role.RolePrivileges.Select(
                    p => mapper.Map<PrivilegeDTO>(p.Privilege)).ToList();

                roleDtos.Add(new UserRoleDTO()
                {
                    Role = roleDto,
                    IsActive = userRole.IsActive,
                });
            }
            dto.UserRoles = roleDtos;

            // Get granted privileges of user
            var privilegeDtos = new List<UserPrivilegeDTO>();
            foreach (var userPrivilege in user.UserPrivileges)
            {
                var privilegeDto = mapper.Map<PrivilegeDTO>(userPrivilege.Privilege);

                privilegeDtos.Add(new UserPrivilegeDTO()
                {
                    Privilege = privilegeDto,
                    IsGranted = userPrivilege.IsGranted,
                });
            }
            dto.UserPrvileges = privilegeDtos;

            return dto;
        }

        public async Task<IEnumerable<UserDTO>> GetUsersAsync(
            string? sortBy,
            QueryUserDTO dto,
            string createdBy,
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

            if (!users.Any())
                throw new UserNotFound();

            // Map to DTO
            return users.Select(user => mapper.Map<UserDTO>(user));
        }

        public async Task<UserDTO> CreateUserAsync(
            UserCreateDTO dto,
            string createdBy,
            string role)
        {
            await unitOfWork.BeginTransactionAsync();

            var existingByEmail = await unitOfWork
                .GetRepository<IUserRepository>()
                .ExistsByEmailAsync(dto.Email);
            if (existingByEmail)
                throw new UserAlreadyExists(
                    "Email already registered.");

            var existingByPhone = await unitOfWork
                .GetRepository<IUserRepository>()
                .ExistsByPhoneAsync(dto.PhoneNumber);
            if (existingByPhone)
                throw new UserAlreadyExists(
                    "Phone number already registered.");

            var existingByIdentity = await unitOfWork
                .GetRepository<IUserRepository>()
                .ExistsByIdentityNumberAsync(dto.IdentityNumber);
            if (existingByIdentity)
                throw new UserAlreadyExists(
                    "Identity number already registered.");

            if (dto.RoleCodes == null || dto.RoleCodes.Count == 0)
                throw new InvalidUserAggregateException(
                    "User must have at least one role.");

            var user = new User(
                userID: Guid.NewGuid(),
                email: dto.Email,
                fullName: dto.FullName,
                password: dto.Password,
                dob: dto.DateOfBirth,
                address: dto.Address,
                gender: dto.Gender,
                phone: dto.PhoneNumber,
                identityNumber: dto.IdentityNumber,
                createdBy: createdBy,
                isActive: true
            );

            foreach (var code in dto.RoleCodes)
            {
                var r = await unitOfWork
                .GetRepository<IRoleRepository>()
                    .GetByCodeAsync(code);

                if (r == null)
                    throw new InvalidUserAggregateException(
                        $"Role code '{code}' does not exist.");

                // Validate permited set role
                ValidateRoleModifying(role, r);

                user.AddRole(r.RoleID);
            }

            unitOfWork
                .GetRepository<IUserRepository>()
                .Add(user);
            await unitOfWork.CommitAsync(dto.PerformedBy);

            return mapper.Map<UserDTO>(user);
        }

        public async Task DeleteUserAsync(
            Guid userId,
            UserDeleteDTO dto,
            string createdBy,
            string role)
        {
            var u = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByIdAsync(userId);

            var user = ValidateAccountModifying(createdBy, u, role);

            await iAMDeletePublisher.PublishAsync(new IAMRequestDeleteMBDTO()
            {
                IdentityNumber = user.IdentityNumber
            });

            await unitOfWork.BeginTransactionAsync();
            user.UpdateActive(false);
            unitOfWork
                .GetRepository<IUserRepository>()
                .Update(userId, user);
            await unitOfWork.CommitAsync(dto.PerformBy);
        }

        public async Task<UserDTO> UpdateUserAsync(
            Guid userId,
            UserUpdateDTO dto,
            string createdBy,
            string role)
        {
            await unitOfWork.BeginTransactionAsync();

            var u = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByDetailByIdAsync(userId);

            var user = ValidateAccountModifying(createdBy, u, role);

            // Check if another user contains the email (excluding current user)
            var existingByEmail = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetAllAsync();
            if (existingByEmail.Any(u => u.UserID != userId && u.Email.Contains(
                dto.Email,
                StringComparison.OrdinalIgnoreCase)))
            {
                throw new UserAlreadyExists("Email already registered.");
            }

            // Check if another user contains the phone number (excluding current user)
            if (existingByEmail.Any(u => u.UserID != userId && u.Phone.Contains(dto.PhoneNumber)))
            {
                throw new UserAlreadyExists("Phone number already registered.");
            }

            // Update fields
            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.UpdateFullName(dto.FullName);

            if (!string.IsNullOrWhiteSpace(dto.Address))
                user.UpdateAddress(dto.Address);

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                user.UpdatePhone(dto.PhoneNumber);

            if (!string.IsNullOrWhiteSpace(dto.Gender))
                user.UpdateGender(dto.Gender);

            if (!string.IsNullOrWhiteSpace(dto.Email))
                user.UpdateEmail(dto.Email);

            if (dto.DateOfBirth != null)
                user.UpdateDob(dto.DateOfBirth.Value);

            var privilegeUpdates = dto.UserPrivilegeUpdateDTOs?
                .Select(p => (p.IsGranted, p.PrivilegeID))
                .ToList() ?? new List<(bool IsGranted, Guid PrivilegeID)>();

            // Validate permitted update privilege
            ValidatePermittedPrivilege(user, privilegeUpdates, role);

            await unitOfWork
                .GetRepository<IUserRepository>()
                .UpdateUserPrivilegesAsync(userId, privilegeUpdates);

            // Validate permited update role
            if (dto.UserRoleUpdateDTOs?.Any() == true)
            {
                foreach (var rId in dto.UserRoleUpdateDTOs)
                {
                    var r = await unitOfWork.GetRepository<IRoleRepository>()
                        .GetByIdAsync(rId.RoleID);

                    if (r == null)
                        throw new InvalidUserAggregateException(
                            $"Role with id: '{rId}' does not exist.");

                    ValidateRoleModifying(role, r);
                }
            }

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

            unitOfWork
                .GetRepository<IUserRepository>()
                .Update(user.UserID, user);
            await unitOfWork.CommitAsync(dto.PerformedBy);

            await iAMUpdatePublisher.PublishAsync(new IAMRequestUpdateMBDTO()
            {
                IdentityNumber = user.IdentityNumber,
                Address = dto.Address,
                DateOfBirth = dto.DateOfBirth,
                FullName = dto.FullName,
                Gender = dto.Gender,
                PhoneNumber = dto.PhoneNumber
            });

            return mapper.Map<UserDTO>(user);
        }

        public async Task ChangePasswordAsync(string identityNumber, ChangePasswordDTO dto)
        {
            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByIdentityNumberAsync(identityNumber);

            if (user == null)
                throw new UserNotFound();

            if (dto.NewPassword != dto.NewConfirmedPassword)
                throw new InvalidChangePassword(
                    "New password and confirmed password are not matched.");

            if (!user.Password.Verify(dto.OldPassword))
                throw new InvalidChangePassword(
                    "Old password does not matched.");

            await unitOfWork.BeginTransactionAsync();
            user.ChangePassword(dto.NewPassword);
            unitOfWork
                .GetRepository<IUserRepository>()
                .Update(user.UserID, user);
            await unitOfWork.CommitAsync(dto.PerformedBy);
        }

        public async Task PatientSyncUpdating(IAMConsumeUpdateDTO dto)
        {
            await unitOfWork.BeginTransactionAsync();

            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByIdentityNumberAsync(dto.IdentityNumber)
                ?? throw new UserNotFound();

            // Update fields
            if (!string.IsNullOrWhiteSpace(dto.FullName))
                user.UpdateFullName(dto.FullName);

            if (!string.IsNullOrWhiteSpace(dto.Address))
                user.UpdateAddress(dto.Address);

            if (!string.IsNullOrWhiteSpace(dto.PhoneNumber))
                user.UpdatePhone(dto.PhoneNumber);

            if (!string.IsNullOrWhiteSpace(dto.Gender))
                user.UpdateGender(dto.Gender);

            if (dto.DateOfBirth != null)
                user.UpdateDob(dto.DateOfBirth.Value.ToDateTime(TimeOnly.MinValue));

            unitOfWork
                .GetRepository<IUserRepository>()
                .Update(user.UserID, user);
            await unitOfWork.CommitAsync(dto.PerformBy);
        }

        public async Task PatientSyncDeleting(IAMConsumeDeleteDTO dto)
        {
            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByIdentityNumberAsync(dto.IdentityNumber);
            if (user == null)
                throw new UserNotFound();

            var patientRole = await unitOfWork
                .GetRepository<IRoleRepository>()
                .GetByCodeAsync(RoleKey.NORMAL_USER);
            if (patientRole == null)
                throw new RoleCodeNotFound(
                    $"Role code {RoleKey.NORMAL_USER} is not found");

            var updatedRoles = new List<(bool IsActive, Guid RoleId)>
            {
                (false, patientRole.RoleID)
            };

            await unitOfWork
                .GetRepository<IUserRepository>()
                .UpdateUserRolesAsync(user.UserID, updatedRoles);

            await unitOfWork.CommitAsync(dto.PerformBy);
        }
        #endregion

        #region Private Helpers
        private void ValidateRoleModifying(string userRole, Role role)
        {
            // Make sure lab manager always create lab user account
            if (userRole == RoleKey.LAB_MANAGER && role.Code != RoleKey.LAB_USER)
                throw new InvalidOwner(
                    $"Lab manager can only create lab user account");

            // Make sure admin can not create account that has higher nor equal permission as ADMIN or SUPER_ADMIN
            if (userRole == RoleKey.ADMIN && (
                role.Code == RoleKey.ADMIN ||
                role.Code == RoleKey.SUPER_ADMIN))
                throw new InvalidOwner(
                    $"User has no permission not set nor update an account to {role.Name}");

            // Super admin can set any role
        }

        private User ValidateAccountModifying(string createdBy, User? user, string role)
        {
            if (user == null)
                throw new UserNotFound();

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
