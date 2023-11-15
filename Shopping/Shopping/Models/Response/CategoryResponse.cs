using Shopping.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Shopping.Models.Response
{
    public class CategoryResponse
    {
       public int Id { get; set; }

        [Display(Name = "Country")]
        [MaxLength(100, ErrorMessage = "The field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Name { get; set; }

        public List<ProductResponse> Products { get; set; }

        
    }
}
