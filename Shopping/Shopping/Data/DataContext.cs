using Microsoft.EntityFrameworkCore;
using Shopping.Data.Entities;

namespace Shopping.Data
{
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext>options): base(options)
        {

        }
        public DbSet<Category> Categories { get; set; } 
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<State> states { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Country>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<Category>().HasIndex(c=>c.Name).IsUnique();
            modelBuilder.Entity<City>().HasIndex(c => c.Name).IsUnique();
            modelBuilder.Entity<State>().HasIndex(c => c.Name).IsUnique();
        }
    }
}
