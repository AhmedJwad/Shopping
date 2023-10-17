using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class ResetPasswordViewModel
    {
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "You must enter a valid email.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string UserName { get; set; }

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "The field {0} is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The field {0} must be between {2} and {1} characters.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "The new password and the confirmation are not the same.")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "The field {0} is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The field {0} must be between {2} and {1} characters.")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Field {0} is required.")]
        public string Token { get; set; }

    }
}
