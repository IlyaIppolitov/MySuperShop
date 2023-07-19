using Microsoft.EntityFrameworkCore;

namespace MyShopBackend.Data;

public class AccountRepositoryEf : EfRepository<Account>, IAccountRepository
{
    public AccountRepositoryEf(AppDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Account> GetAccountByEmail(string email, CancellationToken cancellationToken)
    {
        if (email == null) 
            throw new ArgumentNullException(nameof(email));
        
        return await Entities.SingleAsync(e => e.Email == email, cancellationToken);
    }
}