using Application.DTO;

namespace Application.Interface.IService
{
    public interface IAuthService
    {
        Task<TokenDTO> Login(LoginDTO registerDTO);

        Task Register(RegisterDTO loginDTO);
        Task ForgotPasswordAsync(string email);
        Task ResetPasswordAsync(ResetPasswordDTO dto);
        Task<TokenDTO> RefreshAccessToken(RefreshTokenDTO dto);
        Task Logout(string email);
    }
}
