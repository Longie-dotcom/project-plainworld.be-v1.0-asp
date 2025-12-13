namespace Application.DTO
{
    // DTO
    public class UserDTO
    {
        public Guid UserID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public DateTime Dob { get; set; }
        public bool IsActive { get; set; }
        public Guid CreatedBy { get; private set; }
    }

    // Detail
    public class UserDetailDTO
    {
        public Guid UserID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public DateTime Dob { get; set; }
        public bool IsActive { get; set; }
        public Guid CreatedBy { get; private set; }

        public List<UserRoleDTO> UserRoles { get; set; } = new List<UserRoleDTO>();
        public List<UserPrivilegeDTO> UserPrivileges { get; set; } = new List<UserPrivilegeDTO>();

    }

    public class UserRoleDTO
    {
        public Guid UserRoleID { get; set; }
        public Guid RoleID { get; set; }
        public Guid UserID { get; set; }
        public bool IsActive { get; set; }

        public RoleDetailDTO Role { get; set; } = new RoleDetailDTO();
    }

    public class UserPrivilegeDTO
    {
        public Guid UserPrivilegeID { get; set; }
        public Guid PrivilegeID { get; set; }
        public Guid UserID { get; set; }
        public bool IsGranted { get; set; }

        public PrivilegeDTO Privilege { get; set; } = new PrivilegeDTO();
    }

    // Query
    public class QueryUserDTO
    {
        public int PageIndex { get; set; } = 1;
        public int PageLength { get; set; } = 10;
        public string? Search { get; set; } = string.Empty;
        public string? Gender { get; set; } = string.Empty;
        public bool? IsActive { get; set; }
        public DateTime? DateOfBirthFrom { get; set; }
        public DateTime? DateOfBirthTo { get; set; }
    }

    // Create
    public class UserCreateDTO
    {
        public List<string> RoleCodes { get; set; } = new List<string>();
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string IdentityNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime Dob { get; set; }
        public string Password { get; set; } = string.Empty;
    }

    // Update
    public class UserUpdateDTO
    {
        public string? FullName { get; set; } = string.Empty;
        public DateTime? Dob { get; set; }
        public string? Gender { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
    }

    public class UserRoleUpdateDTO
    {
        public List<UserRoleUpdateItem> Items { get; set; } = new List<UserRoleUpdateItem>();
    }

    public class UserRoleUpdateItem
    {
        public Guid RoleID { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserPrivilegeUpdateDTO
    {
        public List<UserPrivilegeUpdateItem> Items { get; set; } = new List<UserPrivilegeUpdateItem>();
    }

    public class UserPrivilegeUpdateItem
    {
        public Guid PrivilegeID { get; set; }
        public bool IsGranted { get; set; }
    }

    public class ChangePasswordDTO
    {
        public string NewPassword { get; set; } = string.Empty;
        public string NewConfirmedPassword { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
    }
}
