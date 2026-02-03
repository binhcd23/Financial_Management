using Financial_Management_Server.DTOs;

namespace Financial_Management_Server.Interfaces.Account
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest);
        Task<UserDto?> GetUserProfileAsync(int userId);
        Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto);
        Task SendOtpAsync(string email);
        Task<bool> ResendConfirmationAsync(string email);
        Task<bool> VerifyOtpAsync(string email, string otpCode);
        Task<ForgotPasswordResponseDto> ForgotPasswordAsync(string email);
        Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request);
        Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordRequest request);
        Task<UserResponsess> UpdateProfileAsync(UserDto dto);
    }
}
