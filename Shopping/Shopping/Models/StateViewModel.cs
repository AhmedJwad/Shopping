using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class StateViewModel
    {
        public int Id { get; set; }

        [Display(Name = "State")]
        [MaxLength(100, ErrorMessage = "The field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Name { get; set; }

        public int CountryId { get; set; }  
    }
}
