using Microsoft.EntityFrameworkCore;
using mysqltosqlserver.Data.Entities;

namespace mysqltosqlserver.Data
{
    public class DataContext:DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)

        {

        }
        public DbSet<persons> personssqlserver { get; set; }
    }
    public class MySQLDbContext : DbContext
    {
        public MySQLDbContext(DbContextOptions<DataContext> options) : base(options)

        {

        }
        public DbSet<personsmysql> personsmysql { get; set; }
    }
}
