using System.Security.Claims;

namespace Financial_Management_Server.Extensions
{
    public static class ClaimsExtensions
    {
        public static int GetUserId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("UserId");
            return claim != null && int.TryParse(claim.Value, out int userId) ? userId : 0;
        }
    }
}
