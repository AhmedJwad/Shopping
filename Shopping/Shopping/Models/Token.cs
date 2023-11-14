using Shopping.Data.Entities;

namespace Shopping.Models
{
    public class Token
    {
        public string token { get; set; }

        public DateTime Expiration { get; set; }

        public User user { get; set; }


    }
}
