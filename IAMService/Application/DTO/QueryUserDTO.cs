namespace Application.DTO
{
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
}
