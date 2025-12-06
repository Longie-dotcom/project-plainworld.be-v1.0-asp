namespace Application.DTO
{
    public class UserCreateDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
        public List<string> RoleCodes { get; set; } = new List<string>();
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string IdentityNumber { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public string Password { get; set; } = string.Empty;
    }
}
