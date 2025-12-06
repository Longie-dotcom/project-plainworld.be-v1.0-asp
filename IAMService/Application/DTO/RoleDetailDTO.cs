namespace Application.DTO
{
    public class RoleDetailDTO
    {
        public Guid RoleID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public List<PrivilegeDTO> Privileges { get; set; } 
            = new List<PrivilegeDTO>();
    }
}
