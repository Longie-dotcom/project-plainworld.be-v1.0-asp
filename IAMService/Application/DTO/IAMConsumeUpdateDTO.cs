namespace Application.DTO
{
    public class IAMConsumeUpdateDTO
    {
        public string IdentityNumber { get; set; } = string.Empty;
        public string PerformBy { get; set; } = string.Empty;

        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public DateOnly? DateOfBirth { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Address { get; set; }
    }
}
