using Financial_Management_Server.Models;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Financial_Management_Server.DTOs
{
    public class LoginRequestDto
    {
        [Required]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6-100 ký tự")]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; } = false;
    }

    public class LoginResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
    }
    public class UserDto
    {
        [Required]
        public int UserId { get; set; }
        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        public string? UserName { get; set; }

        public string? Email { get; set; }
        [Required(ErrorMessage = "Họ và tên không được để trống")]
        [RegularExpression(@"^[\p{L}\s]+$", ErrorMessage = "Họ và tên chỉ được chứa chữ cái và khoảng trắng")]
        public string FullName { get; set; } = string.Empty;
        public DateTime? CreatedDate { get; set; }
        public UserDto() { }
        public UserDto(User user)
        {
            UserId = user.Id;
            UserName = user.UserName;
            Email = user.Email;
            FullName = user.Fullname;
            CreatedDate = user.CreatedAt;
        }
    }
    public class UserResponsess
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
    }

    public class RegisterRequestDto
    {
        [Required(ErrorMessage = "UserName là bắt buộc")]
        public string UserName { get; set; } = string.Empty;

        [Required(ErrorMessage = "FullName là bắt buộc")]
        [RegularExpression(@"^[\p{L}\s]+$",
        ErrorMessage = "FullName chỉ được chứa chữ cái và khoảng trắng")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Định dạng Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class RegisterResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public UserDto? User { get; set; }
    }

    public class VerifyEmailResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
    }

    public class ForgotPasswordRequestDto
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;
    }

    public class ForgotPasswordResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
    public class VerifyOtpRequestDto
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "OTP là bắt buộc")]
        public string OtpCode { get; set; } = string.Empty;
    }
    public class OtpRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Otp1 { get; set; } = string.Empty;
        public string Otp2 { get; set; } = string.Empty;
        public string Otp3 { get; set; } = string.Empty;
        public string Otp4 { get; set; } = string.Empty;
        public string Otp5 { get; set; } = string.Empty;
        public string Otp6 { get; set; } = string.Empty;
    }

    public class VerifyOtpResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
        public string? ResetToken { get; set; }
    }

    public class ResetPasswordRequestDto
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
        [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        public string NewPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [Compare("NewPassword", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }

    public class ResetPasswordResponseDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
    }

    public class ChangePasswordRequest
    {
        public int UserId { get; set; }
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
    }

    public class ChangePasswordResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = null!;
    }

    public class SendOtpRequest
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }

    public class ResendEmailRequest
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }
    }

    public class VerifyOtpRequest
    {
        [Required]
        [EmailAddress]
        public string? Email { get; set; }

        [Required]
        [RegularExpression("^[0-9]{6}$")]
        public string? OtpCode { get; set; }
    }
}
