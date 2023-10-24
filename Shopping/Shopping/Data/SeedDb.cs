using Microsoft.EntityFrameworkCore;
using Shopping.Data.Entities;
using Shopping.Enum;
using Shopping.Helpers;
using System.Runtime.InteropServices;

namespace Shopping.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;
        private readonly IBlobHelper _blobHelper;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public SeedDb(DataContext context, IUserHelper userHelper , IBlobHelper blobHelper , IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _userHelper = userHelper;
            _blobHelper = blobHelper;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCountriesAsync();
            await CheckCategoriesAsync();
            await CheckRolesAsync();
            await CheckUserAsync( "Ahmed", "Almershady", "Amm380@yahoo.com", "322 311 4620", "iraq babil", "bob.jpg", UserType.Admin);
            await CheckProductsAsync();

        }

        private async Task CheckProductsAsync()
        {
            if(!_context.Products.Any())
            {
                await AddProductAsync("Adidas Barracuda", 270000M, 12F, new List<string>() { "Footwear", "Sports" }, new List<string>() { "adidas_barracuda.png" });
                await AddProductAsync("Adidas Superstar", 250000M, 12F, new List<string>() { "Footwear", "Sports" }, new List<string>() { "Adidas_superstar.png" });
                await AddProductAsync("AirPods", 1300000M, 12F, new List<string>() { "Technology", "Apple" }, new List<string>() { "airpos.png", "airpos2.png" });
                await AddProductAsync("Audifonos Bose", 870000M, 12F, new List<string>() { "Technology" }, new List<string>() { "audifonos_bose.png" });
                await AddProductAsync("Bicicleta Ribble", 12000000M, 6F, new List<string>() { "Sports" }, new List<string>() { "bicicleta_ribble.png" });
                await AddProductAsync("Camisa Cuadros", 56000M, 24F, new List<string>() { "Clothes" }, new List<string>() { "camisa_cuadros.png" });
                await AddProductAsync("Casco Bicicleta", 820000M, 12F, new List<string>() { "Sports" }, new List<string>() { "casco_bicicleta.png", "casco.png" });
                await AddProductAsync("iPad", 2300000M, 6F, new List<string>() { "Technology", "Apple" }, new List<string>() { "ipad.png" });
                await AddProductAsync("iPhone 13", 5200000M, 6F, new List<string>() { "Technology", "Apple" }, new List<string>() { "iphone13.png", "iphone13b.png", "iphone13c.png", "iphone13d.png" });
                await AddProductAsync("Mac Book Pro", 12100000M, 6F, new List<string>() { "Technology", "Apple" }, new List<string>() { "mac_book_pro.png" });
                await AddProductAsync("Mancuernas", 370000M, 12F, new List<string>() { "Sports" }, new List<string>() { "mancuernas.png" });
                await AddProductAsync("Mascarilla Cara", 26000M, 100F, new List<string>() { "Beauty" }, new List<string>() { "mascarilla_cara.png" });
                await AddProductAsync("New Balance 530", 180000M, 12F, new List<string>() { "Footwear", "Sports" }, new List<string>() { "newbalance530.png" });
                await AddProductAsync("New Balance 565", 179000M, 12F, new List<string>() { "Footwear", "Sports" }, new List<string>() { "newbalance565.png" });
                await AddProductAsync("Nike Air", 233000M, 12F, new List<string>() { "Footwear", "Sports" }, new List<string>() { "nike_air.png" });
                await AddProductAsync("Nike Zoom", 249900M, 12F, new List<string>() { "Footwear", "Sports" }, new List<string>() { "nike_zoom.png" });
                await AddProductAsync("Buso Adidas Mujer", 134000M, 12F, new List<string>() { "Clothes", "Sports" }, new List<string>() { "buso_adidas.png" });
                await AddProductAsync("Suplemento Boots Original", 15600M, 12F, new List<string>() { "Nutrition" }, new List<string>() { "Boost_Original.png" });
                await AddProductAsync("Whey Protein", 252000M, 12F, new List<string>() { "Nutrition" }, new List<string>() { "whey_protein.png" });
                await AddProductAsync("Arnes Mascota", 25000M, 12F, new List<string>() { "Pets" }, new List<string>() { "arnes_mascota.png" });
                await AddProductAsync("Cama Mascota", 99000M, 12F, new List<string>() { "Pets" }, new List<string>() { "cama_mascota.png" });
                await AddProductAsync("Teclado Gamer", 67000M, 12F, new List<string>() { "Gamer", "Technology" }, new List<string>() { "teclado_gamer.png" });
                await AddProductAsync("Silla Gamer", 980000M, 12F, new List<string>() { "Gamer", "Technology" }, new List<string>() { "silla_gamer.png" });
                await AddProductAsync("Mouse Gamer", 132000M, 12F, new List<string>() { "Gamer", "Technology" }, new List<string>() { "mouse_gamer.png" });
                await _context.SaveChangesAsync();

            }
        }

        private async Task AddProductAsync(string name, decimal price, float stock, List<string> categories, List<string> images)
        {
            Product product = new()
            {
                Name = name,
                Price = price,
                Stock = stock,
                ProductCategories = new List<ProductCategory>(),
                ProductImages=new List<ProductImage>(),

            };
            foreach (string? category in categories)
            {
                product.ProductCategories.Add(new ProductCategory
                {
                    Category=await _context.Categories.FirstOrDefaultAsync(x=>x.Name==category),
                });
            }

            //foreach (string image in images)
            //{
            //    string filePath;
            //    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            //    {

            //        filePath = $"{Environment.CurrentDirectory}\\Images\\products\\{image}";
            //        filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images/products/" + image);

            //    }
            //    else
            //    {
            //        filePath = $"{Environment.CurrentDirectory}/Images/products/{image}";
            //        filePath = Path.Combine(_webHostEnvironment.ContentRootPath, "Images/products/" + image);

            //    }
               // string imageId = await _blobHelper.UploadBlobAsync(filePath, "products");
               // product.ProductImages.Add(new ProductImage { ImageId = imageId });

            //}
            _context.Products.Add(product);
            
        }

        private async Task<User> CheckUserAsync( string FirstName, string Lastname, string Email, string Phonenumber, string Address,
            string imageID, UserType usertype)
        {
            User user = await _userHelper.GetUserAsync(Email);
            if (user == null)
            {
                user = new User
                {
                    FirstName=FirstName,
                    LastName=Lastname,
                    Email=Email,
                    PhoneNumber=Phonenumber,
                    Address=Address,
                    City=_context.Cities.FirstOrDefault(),
                    UserType=usertype,
                    UserName=Email,
                    ImageId=imageID,
                };
                await _userHelper.AddUserAsync(user, "123456");
                await _userHelper.AddUsertoRoleAsync(user, usertype.ToString());
                string token = await _userHelper.GenerateEmailConfirmationTokenAsync(user);
                await _userHelper.ConfirmEmailAsync(user, token);

            }
            return user;
        }

        private async Task CheckRolesAsync()
        {
            await _userHelper.CheckRoleAsync(UserType.Admin.ToString());
            await _userHelper.CheckRoleAsync(UserType.User.ToString());
        }

        private async Task  CheckCategoriesAsync()
        {
           if(!_context.Categories.Any())
            {
                _context.Categories.Add(new Category { Name = "Technology" });
                _context.Categories.Add(new Category { Name = "Clothes" });
                _context.Categories.Add(new Category { Name = "Gamer" });
                _context.Categories.Add(new Category { Name = "Beauty" });
                _context.Categories.Add(new Category { Name = "Nutrition" });
                _context.Categories.Add(new Category { Name = "Footwear" });
                _context.Categories.Add(new Category { Name = "Sports" });
                _context.Categories.Add(new Category { Name = "Pets" });
                _context.Categories.Add(new Category { Name = "Apple" });

            }
            await _context.SaveChangesAsync();   
        }

        private async Task CheckCountriesAsync()
        {
            if (!_context.Countries.Any())
            {
                _context.Countries.Add(new Country
                {
                    Name = "Iraq",
                    States = new List<State>()
                    {
                        new State()
                        {
                            Name = "Babylon",
                            Cities = new List<City>() {
                                new City() { Name = "Al  karama" },
                                new City() { Name = "40 street" },
                                new City() { Name = "Babil" },
                                new City() { Name = "60 streeet" },
                                new City() { Name = "hay Alhussein" },
                            }
                        },
                        new State()
                        {
                            Name = "Baghdad",
                            Cities = new List<City>() {
                                new City() { Name = "Almounsor" },
                                new City() { Name = "hay aljamah" },
                                new City() { Name = "Zaywna" },
                                new City() { Name = "pronces street" },
                                new City() { Name = "Abu jafaer Almounsor" },
                            }
                        },
                    }
                });
                _context.Countries.Add(new Country
                {
                    Name = "United States",
                    States = new List<State>()
                    {
                        new State()
                        {
                            Name = "Florida",
                            Cities = new List<City>() {
                                new City() { Name = "Orlando" },
                                new City() { Name = "Miami" },
                                new City() { Name = "Tampa" },
                                new City() { Name = "Fort Lauderdale" },
                                new City() { Name = "Key West" },
                            }
                        },
                        new State()
                        {
                            Name = "Texas",
                            Cities = new List<City>() {
                                new City() { Name = "Houston" },
                                new City() { Name = "San Antonio" },
                                new City() { Name = "Dallas" },
                                new City() { Name = "Austin" },
                                new City() { Name = "El Paso" },
                            }
                        },
                    }
                });
            }

            await _context.SaveChangesAsync();

        }
    }
}
