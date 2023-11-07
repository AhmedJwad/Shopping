using System.ComponentModel.DataAnnotations;

namespace Shopping.Data.Entities
{
    public class Country
    {
        public int Id { get; set; }

        [Display(Name = "Country")]
        [MaxLength(100, ErrorMessage = "The field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Name { get; set; }

        public ICollection<State> States { get; set; }

        [Display(Name = "States")]
        public int StatesNumber =>States==null ? 0 : States.Count;

        [Display(Name = "Cities")]
        public int CitiesNumber => States == null ? 0 : States.Sum(s => s.CitiesNumber);

    }
}
