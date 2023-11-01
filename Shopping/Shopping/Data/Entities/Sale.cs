using Shopping.Enum;
using System.ComponentModel.DataAnnotations;

namespace Shopping.Data.Entities
{
    public class Sale
    {
        public int Id { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd hh:mm tt}")]
        [Display(Name = "Date")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public DateTime Date { get; set; }

        public User User { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Remarks")]
        public string? Remarks { get; set; }

        public OrderStatus OrderStatus { get; set; }

        public ICollection<SaleDetail> SaleDetails { get; set; }

        [DisplayFormat(DataFormatString = "{0:N0}")]
        [Display(Name = "Sales")]
        public int Lines => SaleDetails == null ? 0 : SaleDetails.Count;

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Quantity")]
        public float Quantity => SaleDetails == null ? 0 : SaleDetails.Sum(sd => sd.Quantity);

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Value")]
        public decimal Value => SaleDetails == null ? 0 : SaleDetails.Sum(sd => sd.Value);

    }
}
