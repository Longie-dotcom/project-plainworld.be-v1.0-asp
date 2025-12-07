namespace PlainWorld.MessageBroker
{
    public class UserDeleteRequestDTO
    {
        public Guid UserID { get; set; }
    }

    public class UserUpdateRequestDTO
    {
        public Guid UserID { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? Gender { get; set; }
        public DateTime? Dob { get; set; }
        public bool? IsActive { get; set; }
    }
}
