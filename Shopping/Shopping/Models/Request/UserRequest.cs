using Shopping.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Shopping.Models.Request
{
    public class UserRequest:User
    {
        public int CityId { get; set; }
        public byte[] ImageArray { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        //[Required(ErrorMessage = "the field {0} is required")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The field {0} must have between {2} and {1} characters")]
        public string Password { get; set; } = null;

        [DataType(DataType.Password)]
        [Display(Name = "Password Confirm")]
       // [Required(ErrorMessage = "the field {0} is required")]
        [StringLength(20, MinimumLength = 6, ErrorMessage = "The field {0} must have between {2} and {1} characters")]
        [Compare("Password", ErrorMessage = "Password and confirmation are not the same")]
        public string PasswordConfirm { get; set; } = null!;
    }
}
