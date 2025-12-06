namespace Application.DTO
{
    public class UserUpdateDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
        public List<UserRoleUpdateDTO>? UserRoleUpdateDTOs { get; set; } = new();
        public List<UserPrivilegeUpdateDTO>? UserPrivilegeUpdateDTOs { get; set; } = new();
        public string? FullName { get; set; } = string.Empty;
        public DateTime? DateOfBirth { get; set; }
        public string? Gender { get; set; } = string.Empty;
        public string? Address { get; set; } = string.Empty;
        public string? Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; } = string.Empty;
    }

    public class UserRoleUpdateDTO
    {
        public Guid RoleID { get; set; }
        public bool IsActive { get; set; }
    }

    public class UserPrivilegeUpdateDTO
    {
        public Guid PrivilegeID { get; set; }
        public bool IsGranted { get; set; }
    }
}
