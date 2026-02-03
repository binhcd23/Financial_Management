using Financial_Management_Server.DTOs;
using Financial_Management_Server.Extensions;
using Financial_Management_Server.Interfaces.Account;
using Financial_Management_Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Financial_Management_Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;

        public AuthController(IAuthService authService, ILogger<AuthController> logger, IConfiguration configuration, UserManager<User> userManager)
        {
            _authService = authService;
            _logger = logger;
            _configuration = configuration;
            _userManager = userManager;
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto loginRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new LoginResponseDto
                    {
                        Success = false,
                        Message = "Dữ liệu không hợp lệ"
                    });
                }

                _logger.LogInformation($"Đang xử lý đăng nhập cho user: {loginRequest.UserName}");

                var result = await _authService.LoginAsync(loginRequest);

                if (result.Success)
                {
                    _logger.LogInformation($"Đăng nhập thành công cho user: {loginRequest.UserName}");
                    return Ok(result);
                }

                _logger.LogWarning($"Đăng nhập thất bại cho user: {loginRequest.UserName} - {result.Message}");
                return Unauthorized(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi đăng nhập cho user: {loginRequest.UserName}");
                return StatusCode(500, new LoginResponseDto
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi hệ thống"
                });
            }
        }

        /// <summary>
        /// Lấy thông tin profile user từ JWT token
        /// </summary>
        [HttpGet("profile")]
        [Authorize]
        public async Task<ActionResult<UserDto>> GetProfile()
        {
            try
            {
                int userId = User.GetUserId();
                if(userId == 0)
                {
                    return Unauthorized(new
                    {
                        Success = false,
                        Message = "Phiên đăng nhập không hợp lệ hoặc đã hết hạn. Vui lòng đăng nhập lại."
                    });
                }
                var user = await _authService.GetUserProfileAsync(userId);
                if (user == null)
                {
                    return NotFound(new { Message = "Không tìm thấy thông tin người dùng" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy profile");
                return StatusCode(500, new { Message = "Đã xảy ra lỗi hệ thống" });
            }
        }
        [HttpPost("send-otp")]
        public async Task<IActionResult> SendOtp([FromBody] SendOtpRequest request)
        {
            try
            {
                if (!ModelState.IsValid || string.IsNullOrWhiteSpace(request.Email))
                {
                    _logger.LogWarning("SendOtp: Invalid request data");
                    return BadRequest(new { Success = false, Message = "Dữ liệu không hợp lệ" });
                }

                _logger.LogInformation($"Đang gửi OTP qua email: {request.Email}");

                await _authService.SendOtpAsync(request.Email!);

                return Ok(new { Success = true, Message = "OTP đã được gửi đến email của bạn" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi gửi OTP cho email: {request.Email}");
                return StatusCode(500, new { Success = false, Message = "Không thể gửi OTP. Vui lòng thử lại sau." });
            }
        }
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtpRegister([FromBody] VerifyOtpRequest request)
        {
            try
            {
                if (!ModelState.IsValid ||
                    string.IsNullOrWhiteSpace(request.Email) ||
                    string.IsNullOrWhiteSpace(request.OtpCode))
                {
                    return BadRequest(new { Success = false, Message = "Dữ liệu không hợp lệ" });
                }

                var isValid = await _authService.VerifyOtpAsync(request.Email!, request.OtpCode!);

                return isValid
                    ? Ok(new { Success = true, Message = "Xác thực OTP thành công" })
                    : BadRequest(new { Success = false, Message = "Mã OTP không chính xác hoặc đã hết hạn" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi xác thực OTP cho email: {request.Email}");
                return StatusCode(500, new { Success = false, Message = "Đã xảy ra lỗi hệ thống" });
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new { Success = false, Message = "Dữ liệu không hợp lệ" });

                var result = await _authService.RegisterAsync(dto);

                if (!result.Success)
                    return BadRequest(result);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi đăng ký user: {dto.FullName}");
                return StatusCode(500, new { Success = false, Message = "Đã xảy ra lỗi hệ thống" });
            }
        }

        [HttpPost("forgot-password")]
        public async Task<ActionResult<ForgotPasswordResponseDto>> ForgotPassword([FromBody] ForgotPasswordRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ForgotPasswordResponseDto
                    {
                        Success = false,
                        Message = "Dữ liệu không hợp lệ"
                    });
                }

                var result = await _authService.ForgotPasswordAsync(request.Email);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in forgot password");
                return StatusCode(500, new ForgotPasswordResponseDto
                {
                    Success = false,
                    Message = "Lỗi hệ thống"
                });
            }
        }

        [HttpPost("reset-password")]
        public async Task<ActionResult<ResetPasswordResponseDto>> ResetPassword([FromBody] ResetPasswordRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ResetPasswordResponseDto
                    {
                        Success = false,
                        Message = "Dữ liệu không hợp lệ"
                    });
                }

                var result = await _authService.ResetPasswordAsync(request);

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in reset password");
                return StatusCode(500, new ResetPasswordResponseDto
                {
                    Success = false,
                    Message = "Lỗi hệ thống"
                });
            }
        }
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ChangePasswordResponse
                    {
                        Success = false,
                        Message = "Dữ liệu không hợp lệ"
                    });
                }

                _logger.LogInformation($"Change password request for user ID: {request.UserId}");

                var result = await _authService.ChangePasswordAsync(request);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error changing password for user ID: {request.UserId}");
                return StatusCode(500, new ChangePasswordResponse
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi hệ thống"
                });
            }
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return BadRequest("Người dùng không tồn tại.");

            var result = await _userManager.ConfirmEmailAsync(user, token);

            if (result.Succeeded)
            {
                var frontendLoginUrl = "http://localhost:5002/Account/Login?status=success";
                return Redirect(frontendLoginUrl);
            }

            return BadRequest("Đường dẫn xác thực không hợp lệ hoặc đã hết hạn.");
        }
        [HttpPost("resend-confirm")]
        public async Task<IActionResult> ResendConfirmation([FromBody] ResendEmailRequest dto)
        {
            if (string.IsNullOrEmpty(dto.Email)) return BadRequest("Email không được trống");

            var result = await _authService.ResendConfirmationAsync(dto.Email);

            if (result)
            {
                return Ok(new { Message = "Đã gửi lại email xác nhận thành công!" });
            }

            return BadRequest(new { Message = "Yêu cầu không hợp lệ hoặc tài khoản đã được xác thực." });
        }
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var error = ModelState.Values.SelectMany(v => v.Errors).FirstOrDefault()?.ErrorMessage;
                    return BadRequest(new UserResponsess
                    {
                        Success = false,
                        Message = error ?? "Dữ liệu không hợp lệ"
                    });
                }
                var result = await _authService.UpdateProfileAsync(dto);

                if (!result.Success)
                {
                    return BadRequest(result);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi cập nhật profile");
                return StatusCode(500, new UserResponsess
                {
                    Success = false,
                    Message = "Đã xảy ra lỗi hệ thống cục bộ. Vui lòng thử lại sau."
                });
            }
        }
    }
}
