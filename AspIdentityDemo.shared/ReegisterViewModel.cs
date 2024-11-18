using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AspNetIdentityDemo.shared
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [StringLength(50,MinimumLength =5)]
        public String Email { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public String Passord { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 5)]
        public String ConfirmPassord { get; set; }
    }
}
