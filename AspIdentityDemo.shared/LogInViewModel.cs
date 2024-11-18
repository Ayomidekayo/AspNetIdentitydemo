using System.ComponentModel.DataAnnotations;
namespace AspNetIdentityDemo.shared
{
    public class LogInViewModel
    {
        [Required]
        [StringLength(50)]
        [EmailAddress]
        public String Email { get; set; }
        [Required]
        [StringLength(50,MinimumLength =5)]
        public String Password { get; set; }
    }
}
