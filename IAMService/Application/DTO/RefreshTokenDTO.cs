namespace Application.DTO
{
    public class RefreshTokenDTO
    {
        public string RefreshToken { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string RoleCode { get; set; } = string.Empty;
    }
}
