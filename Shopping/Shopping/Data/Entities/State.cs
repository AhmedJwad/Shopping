using System.ComponentModel.DataAnnotations;

namespace Shopping.Data.Entities
{
    public class State
    {
        public int Id { get; set; }

        [Display(Name = "State")]
        [MaxLength(100, ErrorMessage = "The field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Name { get; set; }

        public Country country { get; set; }

        public ICollection<City> Cities { get; set; }

        public int CitiesNumber => Cities == null ? 0 : Cities.Count;
    }
}
