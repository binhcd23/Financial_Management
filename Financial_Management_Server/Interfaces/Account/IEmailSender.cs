namespace Financial_Management_Server.Interfaces.Account
{
    public interface IEmailSender
    {
        Task SendAsync(string toEmail, string subject, string htmlBody);
        Task SendOtpAsync(string email);
        Task<bool> VerifyAsync(string email, string code);
        Task SendVerificationEmailAsync(string toEmail, string fullname, int userId, string token);
    }
}
