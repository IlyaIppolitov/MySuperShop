using Microsoft.EntityFrameworkCore;
using MySuperShop.Domain.Entities;
using MySuperShop.Domain.Repositories;

namespace MySuperShop.Data.EntityFramework.Repositories;

public class CartRepositoryEf : EfRepository<Cart>, ICartRepository
{
    public CartRepositoryEf(AppDbContext dbContext) : base(dbContext)
    {
    }
    
    public override async Task<Cart> GetById(Guid id, CancellationToken cancellationToken)
        => await Entities
            .Include(it => it.Items)
            .FirstAsync(it => it.Id == id, cancellationToken);
}