namespace Application.DTO
{
    public class PrivilegeDTO
    {
        public Guid PrivilegeID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
