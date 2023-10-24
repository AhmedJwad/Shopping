namespace Shopping.Models
{
    public class HomeViewMode
    {
        public ICollection<ProductsHomeViewModel> Products { get; set; }
        public float Quantity { get; set; }

    }
}
