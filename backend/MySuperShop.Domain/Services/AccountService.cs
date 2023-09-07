using Microsoft.Extensions.Logging;
using MySuperShop.Domain.Entities;
using MySuperShop.Domain.Exceptions;
using MySuperShop.Domain.Repositories;

namespace MySuperShop.Domain.Services;

public class AccountService
{

    private readonly IAccountRepository _accountRepository;
    private readonly IApplicationPasswordHasher _hasher;
    private readonly ILogger<AccountService> _logger;
    private readonly IUnitOfWork _uow;

    public AccountService(
        IAccountRepository accountRepository,
        IApplicationPasswordHasher hasher,
        IUnitOfWork uow,
        ILogger<AccountService> logger)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        _uow = uow ?? throw new ArgumentNullException(nameof(uow));
        _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public virtual async Task Register(
        string name, 
        string email, 
        string password, 
        Role[] roles,
        CancellationToken cancellationToken)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));
        if (email == null) throw new ArgumentNullException(nameof(email));
        if (password == null) throw new ArgumentNullException(nameof(password));
        
        var existedAccount = await _accountRepository.FindAccountByEmail(email, cancellationToken);
        if (existedAccount is not null)
        {
            throw new EmailAlreadyExistsException("Account with given email already exists!", email);
        }
        
        Account account = new Account(Guid.NewGuid(), name, email, EncryptPassword(password), roles);
        Cart cart = new(Guid.NewGuid(), account.Id);
        await _uow.AccountRepository.Add(account, cancellationToken);
        await _uow.CartRepository.Add(cart, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
    }

    private string EncryptPassword(string request)
    {
        var hashedPassword = _hasher.HashPassword(request);
        _logger.LogDebug("Password was hashed {HashedPassword}", hashedPassword);
        return hashedPassword;
    }

    public virtual async Task<Account> Login(string email, string password, CancellationToken cancellationToken)
    {
        if (email == null) throw new ArgumentNullException(nameof(email));
        if (password == null) throw new ArgumentNullException(nameof(password));

        var account = await _accountRepository.FindAccountByEmail(email, cancellationToken);
        if (account is null)
        {
            throw new AccountNotFoundException("Account with given email not found");
        }
        
        var isPasswordValid = _hasher.VerifyHashedPassword(account.HashedPassword, password, out var rehashNeeded);
        if (!isPasswordValid)
        {
            throw new InvalidPasswordException("Invalid password");
        }

        if (rehashNeeded)
        {
            await RehashPassword(password, account, cancellationToken);
        }

        return account;
    }

    private async Task RehashPassword(string password, Account account, CancellationToken cancellationToken)
    {
        account.HashedPassword = EncryptPassword(password);
        await _accountRepository.Update(account, cancellationToken);
    }

    public async Task<Account> GetAccountById(Guid id, CancellationToken cancellationToken)
    {
        return await _accountRepository.GetById(id, cancellationToken);
    }

    public async Task<Account[]?> GetAll(CancellationToken cancellationToken)
    {
        return await _accountRepository.GetAllAccounts(cancellationToken);
    }

    public async Task<Account> UpdateAccount(Guid id, string name, string email, string password, string roles, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetById(id, cancellationToken);
        account.Name = name;
        account.Email = email;
        account.HashedPassword = EncryptPassword(password);
        account.Roles = roles.Split(',').Select(Enum.Parse<Role>).ToArray();
        await _accountRepository.Update(account, cancellationToken);
        return await _accountRepository.GetById(id, cancellationToken);
    }
}