namespace Application.DTO
{
    // DTO
    public class RoleDTO
    {
        public Guid RoleID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    // Detail
    public class RoleDetailDTO
    {
        public Guid RoleID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public List<PrivilegeDTO> RolePrivileges { get; set; } = new List<PrivilegeDTO>();
    }

    public class RolePrivilegeDTO
    {
        public Guid RolePrivilegeID { get; set; }
        public Guid RoleID { get; set; }
        public Guid PrivilegeID { get; set; }
        public bool IsActive { get; set; }

        public PrivilegeDTO Privilege { get; set; } = new PrivilegeDTO();
    }

    // Create
    public class RoleCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public List<Guid> PrivilegeID { get; set; } = new List<Guid>();
    }

    // Update
    public class RoleUpdateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Guid> PrivilegeID { get; set; } = new List<Guid>();
    }
}
