using System.Text.Json.Serialization;

namespace Shopping.Data.Entities
{
    public class ProductCategory
    {
        public int Id { get; set; }
        [JsonIgnore]
        public Product Product { get; set; }
        public Category Category { get; set; }

    }
}
