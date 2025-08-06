using Microsoft.AspNetCore.Identity;

namespace Travel_Authentication.Models
{
    public class User : ApplicationUser
    {
        public string? AccessToken { get; set; }
        public string? RefreshToken { get; set; }
        //public string? msg { get; set; }
        //public bool isSuccessed { get; set; }
        public string role { get; set; }

    }
}
