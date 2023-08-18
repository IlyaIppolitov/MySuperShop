﻿using Microsoft.Extensions.Logging;
using MySuperShop.Domain.Entities;
using MySuperShop.Domain.Exceptions;
using MySuperShop.Domain.Repositories;

namespace MySuperShop.Domain.Services;

public class AccountService
{

    private readonly IAccountRepository _accountRepository;
    private readonly IApplicationPasswordHasher _hasher;
    private readonly ILogger<AccountService> _logger;

    public AccountService(
        IAccountRepository accountRepository,
        IApplicationPasswordHasher hasher,
        ILogger<AccountService> logger)
    {
        _accountRepository = accountRepository ?? throw new ArgumentNullException(nameof(accountRepository));
        _hasher = hasher ?? throw new ArgumentNullException(nameof(hasher));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    
    public virtual async Task Register(string name, string email, string password, CancellationToken cancellationToken)
    {
        if (name == null) throw new ArgumentNullException(nameof(name));
        if (email == null) throw new ArgumentNullException(nameof(email));
        if (password == null) throw new ArgumentNullException(nameof(password));
        
        var existedAccount = await _accountRepository.FindAccountByEmail(email, cancellationToken);
        if (existedAccount is not null)
        {
            throw new EmailAlreadyExistsException("Account with given email already exists!", email);
        }
        
        var account = new Account(name, email, EncryptPassword(password));
        await _accountRepository.Add(account, cancellationToken);
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

    private Task RehashPassword(string password, Account account, CancellationToken cancellationToken)
    {
        account.HashedPassword = EncryptPassword(password);
        return _accountRepository.Update(account, cancellationToken);
    }

    public object GetAccountById(Guid id, CancellationToken cancellationToken)
    {
        return _accountRepository.GetById(id, cancellationToken);
    }
}