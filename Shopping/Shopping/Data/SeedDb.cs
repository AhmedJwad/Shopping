using Shopping.Data.Entities;
using Shopping.Enum;
using Shopping.Helpers;

namespace Shopping.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;
        private readonly IUserHelper _userHelper;

        public SeedDb(DataContext context, IUserHelper userHelper)
        {
            _context = context;
            _userHelper = userHelper;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCountriesAsync();
            await CheckCategoriesAsync();
            await CheckRolesAsync();
            await CheckUserAsync( "Ahmed", "Almershady", "Amm380@yahoo.com", "322 311 4620", "iraq babil", "bob.jpg", UserType.Admin);

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
                _context.Categories.Add(new Entities.Category { Name = "Technology" });
                _context.Categories.Add(new Entities.Category { Name = "Clothes" });
                _context.Categories.Add(new Entities.Category { Name = "Gamer" });
                _context.Categories.Add(new Entities.Category { Name = "Accessories" });
                _context.Categories.Add(new Entities.Category { Name = "Beauty" });
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
