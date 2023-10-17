using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class RecoverPasswordViewModel
    {
        [Display(Name = "Email")]
        [Required(ErrorMessage = "The field {0} is required.")]
        [EmailAddress(ErrorMessage = "You must enter a valid email.")]
        public string Email { get; set; }

    }
}
