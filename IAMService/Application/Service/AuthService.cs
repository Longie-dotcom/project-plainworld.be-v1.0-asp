using Application.ApplicationException;
using Application.DTO;
using Application.Enum;
using Application.Interface.IPublisher;
using Application.Interface.IService;
using AutoMapper;
using Domain.Aggregate;
using Domain.IRepository;
using PlainWorld.MessageBroker;
using System.Security.Claims;

namespace Application.Service
{
    public class AuthService : IAuthService
    {
        #region Attributes
        private readonly IUnitOfWork unitOfWork;
        private readonly ITokenService tokenService;
        private readonly IEmailSendPublisher emailSendPublisher;
        private readonly IMapper mapper;
        private readonly ISignalRPublisher signalRPublisher;
        #endregion

        #region Properties
        #endregion

        public AuthService(
            IUnitOfWork unitOfWork,
            ITokenService tokenService,
            IEmailSendPublisher emailSendPublisher,
            IMapper mapper,
            ISignalRPublisher signalRPublisher)
        {
            this.unitOfWork = unitOfWork;
            this.tokenService = tokenService;
            this.emailSendPublisher = emailSendPublisher;
            this.mapper = mapper;
            this.signalRPublisher = signalRPublisher;
        }

        #region Methods
        public async Task<TokenDTO> Login(LoginDTO loginDTO)
        {
            // Get repositories
            var userRepo = unitOfWork.GetRepository<IUserRepository>();
            var roleRepo = unitOfWork.GetRepository<IRoleRepository>();
            var refreshTokenRepo = unitOfWork.GetRepository<IRefreshTokenRepository>();

            #region Verify user
            var user = await userRepo.GetByEmailAsync(loginDTO.Email);
            if (user == null)
                throw new UserNotFound(
                    "User email is not found");
            if (!user.IsActive)
                throw new UserNotFound(
                    "User account has been deactivated");

            // Verify password
            if (!user.Password.Verify(loginDTO.Password))
                throw new InvalidPassword(
                    "The password provided does not match this account.");
            #endregion

            #region Verify user role
            var role = await roleRepo.GetByCodeAsync(loginDTO.RoleCode);
            if (role == null)
                throw new RoleNotFound(
                    $"The login role: {loginDTO.RoleCode} is not found");

            // Get roles of this user
            var userRoles = user.UserRoles;
            if (userRoles == null || !userRoles.Any())
                throw new InvalidRole(
                    $"User has no role in the system");

            // Match role to user's assigned roles
            var matchedUserRole = userRoles.FirstOrDefault(
                ur => ur.RoleID == role.RoleID && ur.IsActive);
            if (matchedUserRole == null)
                throw new InvalidRole(
                    $"User has no permission to login with this role");
            #endregion

            // Get privileges
            var privileges = user.GetEffectivePrivileges(role.RoleID);

            // Generate access token
            var accessToken = tokenService.GenerateToken(
                privileges,
                user.UserID,
                user.Email,
                user.FullName,
                role.Code
            );

            // Generate and save refresh token
            await unitOfWork.BeginTransactionAsync();
            var refreshToken = await refreshTokenRepo.AddTokenAsync(user.UserID);
            await unitOfWork.CommitAsync(user.Email);

            // Return tokens
            return new TokenDTO
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }

        public async Task Register(RegisterDTO registerDTO)
        {
            // Validate email
            var existingByEmail = await unitOfWork
                .GetRepository<IUserRepository>()
                .ExistsByEmailAsync(registerDTO.Email);
            if (existingByEmail)
                throw new UserAlreadyExists(
                    "Email already registered.");

            // Apply domain
            var userId = Guid.NewGuid();
            var user = new User(
                userID: userId,
                email: registerDTO.Email,
                fullName: registerDTO.FullName,
                password: registerDTO.Password,
                dob: registerDTO.Dob,
                gender: registerDTO.Gender,
                createdBy: userId,
                isActive: true
            );

            // Add normal user role as default
            var r = await unitOfWork
            .GetRepository<IRoleRepository>()
                .GetByCodeAsync(RoleKey.NORMAL_USER);
            if (r == null)
                throw new RoleNotFound(
                    $"Role code '{RoleKey.NORMAL_USER}' does not exist.");
            user.AddRole(r.RoleID);

            // Apply persistence
            await unitOfWork.BeginTransactionAsync();
            unitOfWork
                .GetRepository<IUserRepository>()
                .Add(user);
            await unitOfWork.CommitAsync(userId.ToString());
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByEmailAsync(email);

            // Validate email existence
            if (user == null)
                throw new UserEmailNotFound(
                    $"User with email '{email}' was not found.");

            var token = tokenService.GeneratePasswordResetToken(user, 10);
            var client = Environment.GetEnvironmentVariable("IAM_CLIENT_SIDE");
            var resetLink = $"{client}/reset-password?token={token}";

            // Publish message to send email for reset password
            await emailSendPublisher.SendEmail(
                new EmailMessageDTO
                {
                    ToEmail = user.Email,
                    Subject = "Reset your password",
                    BodyHtml = EmailTemplate.BuildResetPasswordEmail(resetLink),
                    IsHtml = true
                });
        }

        public async Task ResetPasswordAsync(ResetPasswordDTO dto)
        {
            if (dto.NewPassword != dto.ConfirmPassword)
                throw new InvalidResetPassword("Reset password not matched.");

            var principal = tokenService.GetPrincipalFromToken(dto.ResetToken);
            if (principal == null || principal.FindFirst("Purpose")?.Value != "PasswordReset")
                throw new InvalidResetPassword(
                    "Invalid or expired token.");

            var userIdStr = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdStr) || !Guid.TryParse(userIdStr, out var userId))
                throw new InvalidResetPassword(
                    "Invalid token payload.");

            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByIdAsync(userId);
            if (user == null)
                throw new UserNotFound(
                    $"User with user ID: {userIdStr} is not found");

            // Apply domain
            user.ChangePassword(dto.NewPassword);

            // Apply persistence
            await unitOfWork.BeginTransactionAsync();
            unitOfWork
                .GetRepository<IUserRepository>()
                .Update(user.UserID, user);
            await unitOfWork.CommitAsync();
        }

        public async Task<TokenDTO> RefreshAccessToken(RefreshTokenDTO dto)
        {
            #region Verify user and refresh token
            // Verify existed user
            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByEmailAsync(dto.Email);
            if (user == null)
                throw new UserNotFound(
                    $"User with email: {dto.Email} is not found");

            // Verify refresh token
            var refreshedToken = await unitOfWork
                .GetRepository<IRefreshTokenRepository>()
                .GetByTokenAsync(dto.RefreshToken);
            if (refreshedToken == null)
                throw new InvalidTokenException(
                    "Refreshed token is expired or not found");
            #endregion

            #region Verify current login role
            // Get role
            var role = await unitOfWork
                .GetRepository<IRoleRepository>()
                .GetByCodeAsync(dto.RoleCode);
            if (role == null)
                throw new RoleNotFound(
                    $"Role with code: {dto.RoleCode} is not found");

            // Get roles of this user
            var userRoles = user.UserRoles;
            if (userRoles == null || !userRoles.Any())
                throw new InvalidRole(
                    $"User has no role in the system");

            // Match role to user's assigned roles
            var matchedUserRole = userRoles.FirstOrDefault(ur => ur.RoleID == role.RoleID);
            if (matchedUserRole == null)
                throw new InvalidRole(
                    $"User has no permission to login with this role");
            #endregion

            // Get role privileges
            var privileges = user.GetEffectivePrivileges(role.RoleID);

            // Generate access token
            var accessToken = tokenService.GenerateToken(
                privileges,
                user.UserID,
                user.Email,
                user.FullName,
                role.Code
            );

            return new TokenDTO()
            {
                AccessToken = accessToken,
                RefreshToken = refreshedToken,
            };
        }

        public async Task Logout(string email)
        {
            var user = await unitOfWork
                .GetRepository<IUserRepository>()
                .GetByEmailAsync(email);
            if (user == null)
                throw new UserNotFound(
                    $"User with email: {email} is not found");

            await unitOfWork.BeginTransactionAsync();
            await unitOfWork
                .GetRepository<IRefreshTokenRepository>()
                .DeleteTokenAsync(user.UserID);
            await unitOfWork.CommitAsync();

            // Publish message to logout all account
            await signalRPublisher.PublishEnvelop(
                new SignalREnvelope.SignalREnvelope()
                {
                    Method = "logout",
                    Payload = "",
                    SourceService = "IAM Service",
                    Topic = email
                });
        }
        #endregion
    }
}
