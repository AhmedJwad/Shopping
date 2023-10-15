using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class ChangePasswordViewModel
    {
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The field {0} must be between {2} and {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string OldPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "New Password")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The field {0} must be between {2} and {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string NewPassword { get; set; }

        [Compare("NewPassword", ErrorMessage = "The new password and the confirmation are not the same.")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirmation new password")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The field {0} must be between {2} and {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Confirm { get; set; }

    }
}
