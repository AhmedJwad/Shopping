﻿using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Shopping.Data.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Display(Name = "Category")]
        [MaxLength(100, ErrorMessage = "The field {0} must have a maximum of {1} characters.")]
        [Required(ErrorMessage = "The field {0} is required.")]
        public string Name { get; set; }
        [JsonIgnore]
        public ICollection<ProductCategory> ProductCategories { get; set; }

        [Display(Name = "# Products")]
        public int ProductsNumber => ProductCategories == null ? 0 : ProductCategories.Count();


    }
}
