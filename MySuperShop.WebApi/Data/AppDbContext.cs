using Microsoft.EntityFrameworkCore;
using MySuperShop.Domain.Entities;

namespace MyShopBackend.Data
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
