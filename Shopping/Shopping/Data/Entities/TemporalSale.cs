using System.ComponentModel.DataAnnotations;

namespace Shopping.Data.Entities
{
    public class TemporalSale
    {
        
        public int Id { get; set; }      

        public Product Product { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Quantity")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public float Quantity { get; set; }        

        public System.DateTime DateCreated { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Comments")]
        public string? Remarks { get; set; }

    }
}
