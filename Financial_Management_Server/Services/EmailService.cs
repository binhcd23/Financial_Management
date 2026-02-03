using Financial_Management_Server.Interfaces.Account;
using Microsoft.Extensions.Caching.Memory;
using System.Net.Mail;
using System.Net;

namespace Financial_Management_Server.Services
{
    public class EmailService : IEmailSender
    {
        private readonly IMemoryCache _cache;
        private readonly IHostEnvironment _env;
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _cfg;

        public EmailService(IMemoryCache cache, IHostEnvironment env, ILogger<EmailService> logger, IConfiguration cfg)
        {
            _cache = cache;
            _env = env;
            _logger = logger;
            _cfg = cfg;
        }

        public async Task SendAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                var s = _cfg.GetSection("Smtp");
                var host = s["Host"];
                var portStr = s["Port"] ?? "587";
                var user = s["User"];
                var pass = s["Pass"];
                var from = s["From"] ?? user;

                if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
                {
                    _logger.LogError("SMTP configuration is missing. Host: {Host}, User: {User}, Pass: {Pass}", host, user, string.IsNullOrEmpty(pass) ? "empty" : "***");
                    throw new InvalidOperationException("SMTP configuration is incomplete. Please check appsettings.json");
                }

                if (string.IsNullOrEmpty(toEmail))
                {
                    _logger.LogError("To email is empty");
                    throw new ArgumentException("To email cannot be empty", nameof(toEmail));
                }

                if (!int.TryParse(portStr, out var port))
                {
                    port = 587;
                }

                pass = pass?.Replace(" ", "") ?? "";

                _logger.LogInformation("Sending email to {Email} from {From} via {Host}:{Port}", toEmail, from, host, port);

                using var client = new SmtpClient(host, port)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(user, pass),
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    Timeout = 30000
                };

                using var mail = new MailMessage()
                {
                    From = new MailAddress(from ?? user, "FinTrack"),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                mail.To.Add(toEmail);

                await client.SendMailAsync(mail);
                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            }
            catch (SmtpException ex)
            {
                _logger.LogError(ex, "SMTP error sending email to {Email}. StatusCode: {StatusCode}", toEmail, ex.StatusCode);
                throw new Exception($"Không thể gửi email: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {Email}", toEmail);
                throw new Exception($"Lỗi gửi email: {ex.Message}", ex);
            }
        }

        public async Task SendOtpAsync(string email)
        {
            var code = Random.Shared.Next(100000, 999999).ToString();
            _cache.Set($"otp:{email}", code, TimeSpan.FromMinutes(10));

            _logger.LogInformation("[OTP] Generated code for {Email}: {Code}", email, code);

            var sendRealEmail = _cfg.GetSection("Smtp").GetValue<bool>("SendRealEmail", true);
            if (_env.IsDevelopment() && !sendRealEmail)
            {
                _logger.LogWarning("🔐 [DEV MODE] OTP cho {Email}: {Code}", email, code);
                return;
            }

            string subject = $"Mã OTP xác thực/khôi phục";
            string htmlBody = $@"
        <html>
        <body style='font-family: Arial, sans-serif;'>
            <div style='max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #ddd; border-radius: 10px;'>
                <h2 style='color: #333;'>Chào bạn!</h2>
                <p>Mã OTP của bạn là:</p>
                <div style='text-align: center; margin: 30px 0;'>
                    <div style='background-color: #f5f5f5; padding: 20px; border-radius: 5px; display: inline-block;'>
                        <h1 style='color: #4CAF50; margin: 0; letter-spacing: 5px; font-size: 36px;'>{code}</h1>
                    </div>
                </div>
                <p style='color: #666; font-size: 14px;'>Mã này có hiệu lực trong <b>10 phút</b>.</p>
                <p style='color: #999; font-size: 12px; margin-top: 30px;'>Nếu không phải bạn yêu cầu, vui lòng đổi mật khẩu ngay lập tức.</p>
            </div>
        </body>
        </html>";

            try
            {
                await SendAsync(email, subject, htmlBody);
                _logger.LogInformation("✅ Email OTP đã gửi đến {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Lỗi khi gửi email đến {Email}", email);
                throw;
            }
        }

        public Task<bool> VerifyAsync(string email, string code)
        {
            var ok = _cache.TryGetValue<string>($"otp:{email}", out var saved) && saved == code;
            if (ok) _cache.Remove($"otp:{email}");
            return Task.FromResult(ok);
        }
        public async Task SendVerificationEmailAsync(string toEmail, string fullname, int userId, string token)
        {
            var encodedToken = System.Web.HttpUtility.UrlEncode(token);

            var baseUrl = _cfg["AppSettings:BaseUrl"] ?? "https://localhost:5001";

            var verifyLink = $"{baseUrl}/api/Auth/verify-email?userId={userId}&token={encodedToken}";

            string subject = "Xác thực tài khoản FinTrack";
            string body = $@"
        <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; border: 1px solid #e2e8f0; border-radius: 16px; padding: 24px;'>
            <h2 style='color: #10b981; text-align: center;'>Chào mừng {fullname}!</h2>
            <div style='text-align: center; margin: 25px 0;'>
                <a href='{verifyLink}' 
                   style='background-color: #10b981; color: white; padding: 14px 32px; text-decoration: none; border-radius: 10px; font-weight: bold; display: inline-block;'>
                   XÁC THỰC TÀI KHOẢN NGAY
                </a>
            </div>
            <p style='color: #94a3b8; font-size: 13px; text-align: center;'>
                Lưu ý: Liên kết này sẽ hết hạn trong vòng 24 giờ.<br>
                Nếu bạn không thực hiện đăng ký này, hãy bỏ qua email này.
            </p>
        </div>"; 

            await SendAsync(toEmail, subject, body);
        }
    }
}
