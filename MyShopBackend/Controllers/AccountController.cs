using Microsoft.AspNetCore.Mvc;
using MyShopBackend.Data;

namespace MyShopBackend.Controllers;

[Route("account")]
[ApiController]
public class AccountController : Controller
{
    private readonly IAccountRepository _repository;

    public AccountController(IAccountRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(Account account, CancellationToken cancellationToken)
    {
        if (account == null) throw new ArgumentNullException(nameof(account));
        await _repository.Add(account, cancellationToken);
        return Ok();
    }
}