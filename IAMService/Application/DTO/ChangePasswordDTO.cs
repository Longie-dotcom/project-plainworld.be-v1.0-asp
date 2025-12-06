namespace Application.DTO
{
    public class ChangePasswordDTO
    {
        public string PerformedBy { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string NewConfirmedPassword { get; set; } = string.Empty;
        public string OldPassword { get; set; } = string.Empty;
    }
}
