namespace Application.DTO
{
    public class RoleUpdateDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public List<Guid> PrivilegeID { get; set; } = new List<Guid>();
    }
}
