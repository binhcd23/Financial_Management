using Financial_Management_Server.DTOs;
using Financial_Management_Server.Interfaces.Account;
using Financial_Management_Server.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Financial_Management_Server.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IEmailSender _emailSender;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        public AuthService(IAuthRepository authRepository, IEmailSender emailSender, UserManager<User> userManager, IConfiguration configuration)
        {
            _authRepository = authRepository;
            _emailSender = emailSender;
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<ChangePasswordResponse> ChangePasswordAsync(ChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return new ChangePasswordResponse { Success = false, Message = "Không tìm thấy tài khoản" };

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if (result.Succeeded)
                return new ChangePasswordResponse { Success = true, Message = "Đổi mật khẩu thành công" };

            var error = result.Errors.FirstOrDefault()?.Description;
            return new ChangePasswordResponse
            {
                Success = false,
                Message = error ?? "Đổi mật khẩu thất bại. Vui lòng kiểm tra lại mật khẩu cũ."
            };
        }

        public async Task<ForgotPasswordResponseDto> ForgotPasswordAsync(string email)
        {
            var emailUser = _userManager.FindByEmailAsync(email);
            if (string.IsNullOrEmpty(email) || emailUser == null)
                return new ForgotPasswordResponseDto { Success = false, Message = "Email không tồn tại trong hệ thống" };
         
            await SendOtpAsync(email);
            return new ForgotPasswordResponseDto
            {
                Success = true,
                Message = "Mã OTP đã được gửi đến email của bạn. Vui lòng kiểm tra email."
            };
        }

        public async Task<UserDto?> GetUserProfileAsync(int userId)
        {
            var user = await _authRepository.GetUserByIdAsync(userId);
            if (user == null) return null;

            return new UserDto(user);
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto loginRequest)
        {
            var user = await _userManager.FindByNameAsync(loginRequest.UserName);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginRequest.Password))
            {
                if (!user.EmailConfirmed)
                {
                    return new LoginResponseDto
                    {
                        Success = false,
                        Message = "Tài khoản chưa được kích hoạt. Vui lòng kiểm tra Email của bạn."
                    };
                }
                var authClaims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(ClaimTypes.Email, user.Email!),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim("UserId", user.Id.ToString())
                };

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Authentication:Jwt:Key"]));

                var token = new JwtSecurityToken(
                    issuer: _configuration["Authentication:Jwt:Issuer"],
                    expires: DateTime.Now.AddMinutes(double.Parse(_configuration["Authentication:Jwt:ExpiryMinutes"])),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
                );

                return new LoginResponseDto
                {
                    Success = true,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Message = "Đăng nhập thành công",
                    UserId = user.Id,
                    FullName = user.Fullname,
                };
            }

            return new LoginResponseDto { Success = false, Message = "Email hoặc mật khẩu không chính xác" };
        }
        public async Task<RegisterResponseDto> RegisterAsync(RegisterRequestDto dto)
        {
            var emailExists = await _userManager.FindByEmailAsync(dto.Email);
            var nameExists = await _userManager.FindByNameAsync(dto.UserName);
            if (emailExists != null || nameExists != null)
                return new RegisterResponseDto { Success = false, Message = "Email/Tên đăng nhập đã tồn tại!" };
          
            var user = new User
            {
                UserName = dto.UserName,
                Email = dto.Email,
                Fullname = dto.FullName,
                CreatedAt = DateTime.Now
            };

            // UserManager sẽ tự động Hash mật khẩu bằng thuật toán chuẩn
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (result.Succeeded)
            {
                try
                {
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    await _emailSender.SendVerificationEmailAsync(user.Email!, user.Fullname!, user.Id, token);
                }
                catch (Exception ex)
                {
                    return new RegisterResponseDto
                    {
                        Success = false,
                        Message = ex.Message
                    };
                }
                return new RegisterResponseDto { Success = true, Message = "Đăng ký thành công" };
            }

            return new RegisterResponseDto
            {
                Success = false,
                Message = string.Join(", ", result.Errors.Select(e => e.Description))
            };
        }

        public async Task<ResetPasswordResponseDto> ResetPasswordAsync(ResetPasswordRequestDto request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
                return new ResetPasswordResponseDto { Success = false, Message = "Không tìm thấy tài khoản" };

            var removeResult = await _userManager.RemovePasswordAsync(user);
            if (removeResult.Succeeded)
            {
                var addResult = await _userManager.AddPasswordAsync(user, request.NewPassword);
                if (addResult.Succeeded)
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                    return new ResetPasswordResponseDto { Success = true, Message = "Đặt lại mật khẩu thành công." };
                }
            }

            return new ResetPasswordResponseDto { Success = false, Message = "Có lỗi xảy ra trong quá trình đặt lại mật khẩu" };
        }

        public async Task SendOtpAsync(string email) => await _emailSender.SendOtpAsync(email);
        public async Task<bool> VerifyOtpAsync(string email, string otpCode) => await _emailSender.VerifyAsync(email, otpCode);

        public async Task<bool> ResendConfirmationAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null || await _userManager.IsEmailConfirmedAsync(user))
            {
                return false;
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            await _emailSender.SendVerificationEmailAsync(user.Email!, user.Fullname!, user.Id, token);

            return true;
        }
        public async Task<UserResponsess> UpdateProfileAsync(UserDto dto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(dto.UserId.ToString());
                if (user == null)
                {
                    return new UserResponsess { Success = false, Message = "Không tìm thấy người dùng." };
                }

                if (user.UserName != dto.UserName)
                {
                    var existingUser = await _userManager.FindByNameAsync(dto.UserName ?? "N/A");
                    if (existingUser != null)
                    {
                        return new UserResponsess { Success = false, Message = "Tên đăng nhập đã tồn tại." };
                    }
                    user.UserName = dto.UserName;
                    user.NormalizedUserName = dto.UserName?? "N/A".ToUpper();
                }

                user.Fullname = dto.FullName.Trim();
                bool isUpdated = await _authRepository.UpdateAsync(user);

                return new UserResponsess
                {
                    Success = isUpdated,
                    Message = isUpdated ? "Cập nhật thành công!" : "Không có thay đổi nào được thực hiện."
                };
            }
            catch (Exception ex)
            {
                return new UserResponsess { Success = false, Message = $"Lỗi: {ex.Message}" };
            }
        }
    }
}
