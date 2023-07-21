using MySuperShop.Domain.Entities;
using MySuperShop.Domain.Repositories;

namespace MySuperShop.Domain.Services;

public class AccountService
{

    private readonly IAccountRepository _accountRepository;

    public AccountService(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
    }
    
    public async Task Register(string name, string email, string password, CancellationToken cancellationToken)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));
        if (email == null) throw new ArgumentNullException(nameof(email));
        if (password == null) throw new ArgumentNullException(nameof(password));
        
        var existedAccount = await _accountRepository.FindAccountByEmail(email, cancellationToken);
        if (existedAccount is not null)
        {
            throw new InvalidOperationException("Account with given email is already exist");
        }
        
        Account account = new Account(Guid.Empty, name, email, EncryptPassword(password));
        await _accountRepository.Add(account, cancellationToken);
    }

    private static string EncryptPassword(string request)
    {
        return request;
    }
}