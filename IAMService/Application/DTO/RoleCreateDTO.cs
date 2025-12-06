namespace Application.DTO
{
    public class RoleCreateDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
        public List<Guid> PrivilegeID { get; set; } = new List<Guid>();
    }
}
