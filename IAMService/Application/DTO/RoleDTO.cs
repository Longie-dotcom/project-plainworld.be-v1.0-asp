namespace Application.DTO
{
    public class RoleDTO
    {
        public Guid RoleID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public List<Guid> PrivilegeID { get; set; } = new List<Guid>();
    }
}
