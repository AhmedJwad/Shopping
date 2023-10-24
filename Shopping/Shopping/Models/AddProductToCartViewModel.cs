using Shopping.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class AddProductToCartViewModel
    {
        public int Id { get; set; }

        [Display(Name = "Name")]
        [MaxLength(50, ErrorMessage = "The field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Description")]
        [MaxLength(500, ErrorMessage = "The field {0} must have a maximum of {1} characters.")]
        public string Description { get; set; }

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Price")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public decimal Price { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Inventory")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public float Stock { get; set; }

        [Display(Name = "Categories")]
        public string Categories { get; set; }

        public ICollection<ProductImage> ProductImages { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Quantity")]
        [Range(0.0000001, float.MaxValue, ErrorMessage = "You must enter a value greater than zero in the quantity.")]
        [Required(ErrorMessage = "Field {0} is required.")]
        public float Quantity { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Comments")]
        public string? Remarks { get; set; }

    }
}
