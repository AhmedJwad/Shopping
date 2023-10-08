using System.ComponentModel.DataAnnotations;

namespace Shopping.Data.Entities
{
    public class City
    {
        public int Id { get; set; }

        [Display(Name = "City")]
        [MaxLength(100, ErrorMessage = "The field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Name { get; set; }

        public State state { get; set; }
    }
}
