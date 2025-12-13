namespace Application.DTO
{
    // DTO
    public class PrivilegeDTO
    {
        public Guid PrivilegeID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    // Query
    public class QueryPrivilegeDTO
    {
        public int? PageIndex { get; set; }
        public int? PageLength { get; set; }
        public string? Search { get; set; }
    }

    // Create
    public class PrivilegeCreateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    // Update
    public class PrivilegeUpdateDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
