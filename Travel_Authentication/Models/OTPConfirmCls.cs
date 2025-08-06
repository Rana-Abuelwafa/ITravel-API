using System.ComponentModel.DataAnnotations;

namespace Travel_Authentication.Models
{
    public class OTPConfirmCls
    {
        [Required(ErrorMessage = "email is required")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "otp is required")]
        public required string otp { get; set; }
    }
}
