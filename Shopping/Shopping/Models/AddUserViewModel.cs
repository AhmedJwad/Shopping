using Shopping.Enum;
using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class AddUserViewModel:EditUserViewModel
    {
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "You must enter a valid email.")]
        [MaxLength(100, ErrorMessage = "The field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "The field {0} is requiredo.")]
        public string Username { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [Required(ErrorMessage = "The field {0} is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The field {0} must be between {2} and {1} characters.")]
        public string Password { get; set; }

        [Compare("Password", ErrorMessage = "Password and confirmation are not the same.")]
        [Display(Name = "Password confirmation")]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "The field {0} is required.")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The field {0} must be between {2} and {1} characters.")]
        public string PasswordConfirm { get; set; }

        [Display(Name = "Type of user")]
        public UserType UserType { get; set; }

    }
}
