using System.ComponentModel.DataAnnotations;

namespace Shopping.Data.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }

        public Product Product { get; set; }

        [Display(Name = "Photo")]
        public string ImageId { get; set; }       

        //TODO: Pending to change to the correct path
        [Display(Name = "Photo")]
        public string ImageFullPath => ImageId == string.Empty
            ? $"https://localhost:7057/images/noimage.png"
            : $"https://localhost:7237/{ImageId}";

    }
}
