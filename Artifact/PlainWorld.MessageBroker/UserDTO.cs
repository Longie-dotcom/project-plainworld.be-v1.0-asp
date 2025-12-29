namespace PlainWorld.MessageBroker
{
    public class UserDeleteDTO
    {
        public Guid UserID { get; set; }
    }

    public class UserUpdateDTO
    {
        public Guid UserID { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public bool? IsActive { get; set; }
    }

    public class UserCreateDTO
    {
        public Guid UserID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public DateTime Dob { get; set; }
    }
}
