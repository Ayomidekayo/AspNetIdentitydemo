using System.ComponentModel.DataAnnotations;

namespace AspNetIdentityDemo.Api.Model
{
    public class ResetPasswordViewModel
    {
        [Required]
        public string Token { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(50,MinimumLength =5)]
        public string Newpassword { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public string Confirmpassword { get; set; }
    }
}
