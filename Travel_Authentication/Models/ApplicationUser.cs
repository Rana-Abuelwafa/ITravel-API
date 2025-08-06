using Microsoft.AspNetCore.Identity;

namespace Travel_Authentication.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool? sendOffers { get; set; }
        public int completeprofile { get; set; }
        public string? GoogleId { get; set; }
    }
}
