using System.ComponentModel.DataAnnotations;

namespace Travel_Authentication.Models
{
    public class LoginModel
    {
        

        [Required(ErrorMessage = "email is required")]
        public required string Email { get; set; }


        [Required(ErrorMessage = "Password is required")]
        public required string Password { get; set; }

        public required string lang { get; set; }
    }
}
