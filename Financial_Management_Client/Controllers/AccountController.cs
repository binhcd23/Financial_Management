
using Financial_Management_Server.DTOs;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime.Intrinsics.Arm;
using static System.Net.WebRequestMethods;

namespace Financial_Management_Client.Controllers
{
    public class AccountController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;
        private readonly ILogger<AccountController> _logger;
        public AccountController(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AccountController> logger)
        {
            _logger = logger;
            _config = configuration;
            _httpClient = httpClientFactory.CreateClient("default");
        }
        [HttpGet]
        public IActionResult Register() => View(new RegisterRequestDto());

        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequestDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            try
            {
                var resp = await _httpClient.PostAsJsonAsync("api/Auth/register", dto);

                if (resp.IsSuccessStatusCode)
                {
                    ViewBag.ShowSuccessModal = true;
                    return View(dto);
                }
                var result = await resp.Content.ReadFromJsonAsync<RegisterResponseDto>();
                string msg = result?.Message ?? "Đăng ký thất bại.";
                ModelState.AddModelError(string.Empty, msg);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Register Error");
                ModelState.AddModelError(string.Empty, "Lỗi kết nối máy chủ.");
            }

            return View(dto);
        }

        [HttpPost]
        public async Task<IActionResult> ResendConfirmation(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return Json(new { success = false, message = "Email không hợp lệ." });
            }

            try
            {
                
                var response = await _httpClient.PostAsJsonAsync("api/Auth/resend-confirm", new { Email = email });

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Đã gửi lại email xác nhận!" });
                }

                var error = await response.Content.ReadAsStringAsync();
                return Json(new { success = false, message = "Không thể gửi lại: " + error });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi gọi API gửi lại mail");
                return Json(new { success = false, message = "Kết nối máy chủ thất bại." });
            }
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            HttpContext.Session.Clear();
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ErrorMessage"] = "Email không hợp lệ";
                    return View(request);
                }
                var response = await _httpClient.PostAsJsonAsync("api/Auth/forgot-password", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ForgotPasswordResponseDto>();
                    if (result?.Success == true)
                    {
                        TempData["SuccessMessage"] = result.Message;
                        return RedirectToAction(nameof(VerifyOtp), new { email = request.Email });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = result?.Message ?? "Có lỗi xảy ra";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Không thể gửi OTP. Vui lòng thử lại.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in forgot password");
                TempData["ErrorMessage"] = "Lỗi hệ thống. Vui lòng thử lại.";
            }

            return View(request);
        }
        [HttpGet]
        public IActionResult VerifyOtp(string? email)
        {
            if (string.IsNullOrEmpty(email)) return RedirectToAction(nameof(Login));
            ViewBag.Email = email;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> VerifyOtp(OtpRequest otp)
        {
            var otpCode = $"{otp.Otp1}{otp.Otp2}{otp.Otp3}{otp.Otp4}{otp.Otp5}{otp.Otp6}";

            if (!await VerifyOtpAsync(otp.Email, otpCode))
            {
                ViewBag.Email = otp.Email;
                ViewBag.Error = "Mã OTP không chính xác hoặc đã hết hạn.";
                return View();
            }
            return RedirectToAction("SetPassword", new { email = otp.Email });
        }
        private async Task<bool> SendOtpAsync(string email)
        {
            try
            {
                _logger.LogInformation($"Đang gửi OTP cho email: {email}");
                var requestData = new { Email = email };

                var resp = await _httpClient.PostAsJsonAsync("api/Auth/send-otp", requestData);

                _logger.LogInformation($"Response status: {resp.StatusCode}, Success: {resp.IsSuccessStatusCode}");

                if (!resp.IsSuccessStatusCode)
                {
                    var errorStr = await resp.Content.ReadAsStringAsync();
                    _logger.LogError($"Lỗi gửi OTP: Status {resp.StatusCode}, Error: {errorStr}");
                }

                return resp.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception khi gửi OTP cho {email}: {ex.Message}");
                return false;
            }
        }
        [HttpPost]
        public async Task<IActionResult> ResendOtp(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("ForgotPassword");
            }

            try
            {
                await SendOtpAsync(email);

                ViewBag.Email = email;
                ViewBag.ResendSuccess = true;
                return View("VerifyOtp", new { email });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi gửi lại OTP");
                ModelState.AddModelError("", "Không thể gửi lại mã, vui lòng thử lại sau.");
                return View("VerifyOtp", new { email });
            }
        }
        private async Task<bool> VerifyOtpAsync(string email, string code)
        {
            try
            {
                var requestData = new { Email = email, OtpCode = code };
                var resp = await _httpClient.PostAsJsonAsync("api/Auth/verify-otp", requestData);
                return resp.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Exception khi verify OTP cho {email}");
                return false;
            }
        }

        [HttpGet]
        public IActionResult SetPassword(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Thông tin không hợp lệ";
                return RedirectToAction(nameof(ForgotPassword));
            }

            var request = new ResetPasswordRequestDto
            {
                Email = email,
            };
            return View(request);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequestDto request)
        {
            try
            {
                if (!ModelState.IsValid) return View("SetPassword", request);
                var response = await _httpClient.PostAsJsonAsync("api/Auth/reset-password", request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<ResetPasswordResponseDto>();
                    if (result?.Success == true)
                    {
                        TempData["SuccessMessage"] = result.Message;
                        return RedirectToAction("Login");
                    }
                    ModelState.AddModelError("", result?.Message ?? "Có lỗi xảy ra");
                }
                else
                {
                    ModelState.AddModelError("", "Không thể đặt lại mật khẩu. Vui lòng thử lại.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in reset password");
                ModelState.AddModelError("", "Lỗi kết nối máy chủ.");
            }
            return View("SetPassword", request);
        }

        [HttpGet]
        public IActionResult Login(string? status)
        {
            if (status == "success")
            {
                ViewBag.SuccessMessage = "Xác thực email thành công!";
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDto dto)
        {
            if (!ModelState.IsValid) return View(dto);

            try
            {
                var requestData = new { username = dto.UserName, password = dto.Password, rememberMe = dto.RememberMe };
                var response = await _httpClient.PostAsJsonAsync("api/Auth/login", requestData);

                if (response.IsSuccessStatusCode)
                {
                    var loginResult = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                    if (loginResult != null && loginResult.Success)
                    {
                        HttpContext.Session.SetString("JWToken", loginResult.Token);
                        HttpContext.Session.SetString("FullName", loginResult.FullName);
                        HttpContext.Session.SetString("UserId", loginResult.UserId.ToString());
                      
                        return RedirectToAction("Dashboard", "Finance");
                    }
                    ModelState.AddModelError("", loginResult?.Message ?? "Thông tin đăng nhập không đúng.");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    var errorResult = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                    ModelState.AddModelError("", errorResult?.Message ?? "Sai tài khoản hoặc mật khẩu.");
                }
                else
                {
                    ModelState.AddModelError("", "Máy chủ API đang gặp sự cố.");
                }

                return View(dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login Error");
                ModelState.AddModelError("", "Không thể kết nối đến máy chủ.");
                return View(dto);
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        [HttpGet]
        public async Task<IActionResult> MyProfile()
        {
            try
            {
                var token = HttpContext.Session.GetString("JWToken");
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Login");
                }

                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.GetAsync("api/Auth/profile");

                if (response.IsSuccessStatusCode)
                {
                    var userProfile = await response.Content.ReadFromJsonAsync<UserDto>();
                    return View(userProfile);
                }

                TempData["ErrorMessage"] = "Không thể tải thông tin cá nhân (Lỗi API).";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching profile");
                TempData["ErrorMessage"] = "Lỗi kết nối máy chủ.";
            }
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            try
            {
                // 1. Kiểm tra Validation cơ bản
                if (!ModelState.IsValid)
                {
                    ViewData["IsPasswordError"] = true;
                    return View("MyProfile", await GetUserProfileInternal());
                }

                // 2. Gắn Token để gọi API đổi mật khẩu
                var token = HttpContext.Session.GetString("JWToken");
                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.PostAsJsonAsync("api/Auth/change-password", request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                    return RedirectToAction("MyProfile");
                }
                else
                {
                    // 3. Xử lý lỗi từ API trả về (Ví dụ: Mật khẩu cũ không đúng)
                    var errorResult = await response.Content.ReadFromJsonAsync<ChangePasswordResponse>();
                    ModelState.AddModelError("", errorResult?.Message ?? "Lỗi không xác định.");
                    ViewData["IsPasswordError"] = true;
                    // Trả về View MyProfile cùng dữ liệu User để không bị lỗi Model
                    return View("MyProfile", await GetUserProfileInternal());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in change password");
                TempData["ErrorMessage"] = "Lỗi hệ thống. Vui lòng thử lại.";
                ViewData["IsPasswordError"] = true;
                return View("MyProfile", await GetUserProfileInternal());
            }
        }
        private async Task<UserDto> GetUserProfileInternal()
        {
            var token = HttpContext.Session.GetString("JWToken");
            _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            var profileResponse = await _httpClient.GetAsync("api/Auth/profile");
            return await profileResponse.Content.ReadFromJsonAsync<UserDto>() ?? new UserDto();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(UserDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View("MyProfile", dto);
                }

                var response = await _httpClient.PutAsJsonAsync("api/Auth/update-profile", dto);

                if (response.IsSuccessStatusCode)
                {
                    if (!string.IsNullOrEmpty(dto.FullName))
                    {
                        HttpContext.Session.SetString("FullName", dto.FullName);
                    }
                    TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
                    return RedirectToAction("MyProfile");
                }

                var errorResult = await response.Content.ReadFromJsonAsync<UserResponsess>();
                ModelState.AddModelError("", errorResult?.Message ?? "Lỗi từ phía máy chủ API.");

                return View("MyProfile", dto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi cập nhật profile");
                TempData["ErrorMessage"] = "Lỗi kết nối hệ thống.";
                return View("MyProfile", dto);
            }
        }
    }
}
