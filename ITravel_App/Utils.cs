using System.Security.Claims;

namespace ITravel_App
{
    public class Utils
    {
        public static LoginUserData getTokenData(IHttpContextAccessor _httpContextAccessor)
        {
            LoginUserData userData = new LoginUserData();
            string? clientId = string.Empty;
            string? email = string.Empty;
            string? FullName = string.Empty;
            if (_httpContextAccessor.HttpContext is not null)
            {
                clientId = _httpContextAccessor.HttpContext.User.FindFirstValue("ClientId");
                email = _httpContextAccessor.HttpContext.User.FindFirstValue("Email");
                FullName = _httpContextAccessor.HttpContext.User.FindFirstValue("FullName");
            }
            userData.client_id = clientId;
            userData.client_email = email;
            return userData;
        }
    }
}
