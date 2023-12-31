﻿using Shopping.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace Shopping.Models
{
    public class ShowCartViewModel
    {
       

        [DataType(DataType.MultilineText)]
        [Display(Name = "Comments")]
        public string? Remarks { get; set; }     

        public string? username { get; set; }
        public ICollection<TemporalSale> TemporalSales { get; set; }

        [DisplayFormat(DataFormatString = "{0:N2}")]
        [Display(Name = "Amount")]
        public float Quantity => TemporalSales == null ? 0 : TemporalSales.Sum(ts => ts.Quantity);

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Value")]
        public decimal Value => TemporalSales == null ? 0 : TemporalSales.Sum(ts => (decimal)(ts.Product.Price) * (decimal)(ts.Quantity));

        [DisplayFormat(DataFormatString = "{0:C2}")]
        [Display(Name = "Price")]
        public decimal Price => TemporalSales == null ? 0 : TemporalSales.Sum(ts => ts.Value);


    }
}
