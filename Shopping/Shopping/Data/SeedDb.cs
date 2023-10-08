using Shopping.Data.Entities;

namespace Shopping.Data
{
    public class SeedDb
    {
        private readonly DataContext _context;

        public SeedDb(DataContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            await _context.Database.EnsureCreatedAsync();
            await CheckCountriesAsync();
            await CheckCategoriesAsync();

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
