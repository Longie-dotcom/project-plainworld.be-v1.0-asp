namespace Application.DTO
{
    public class UserDetailDTO
    {
        // For details
        public Guid UserID { get; set; }
        public List<UserRoleDTO> UserRoles { get; set; } = new List<UserRoleDTO>();
        public List<UserPrivilegeDTO> UserPrvileges { get; set; } = new List<UserPrivilegeDTO>();

        // For list view
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string IdentityNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Address { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserRoleDTO
    {
        public RoleDetailDTO Role { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserPrivilegeDTO
    {
        public PrivilegeDTO Privilege { get; set; }
        public bool IsGranted { get; set; }
    }
}
