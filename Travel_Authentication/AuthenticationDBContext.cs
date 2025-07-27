using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Travel_Authentication.Models;


namespace Travel_Authentication
{
    public class AuthenticationDBContext : IdentityDbContext<ApplicationUser>    {
        public AuthenticationDBContext(DbContextOptions<AuthenticationDBContext> options)
            : base(options)
        {
        }
    }
}
