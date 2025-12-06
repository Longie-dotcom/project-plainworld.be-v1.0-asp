namespace Application.DTO
{
    public class IAMRequestUpdateMBDTO
    {
        public string IdentityNumber { get; set; } = string.Empty;

        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}
