using Microsoft.EntityFrameworkCore;
using MySuperShop.Domain.Entities;

namespace MySuperShop.Data.EntityFramework
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Account> Accounts => Set<Account>();

        public AppDbContext(
            DbContextOptions<AppDbContext> options) :
            base(options)
        {
        }
    }
}
