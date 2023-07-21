using Microsoft.AspNetCore.Mvc;
using MySuperShop.Domain.Services;
using MySuperShop.HttpModels.Requests;

namespace MyShopBackend.Controllers;

[Route("account")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly AccountService _accountService;

    AccountController(AccountService accountService)
    {
        _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        try
        {
            await _accountService.Register(request.Name, request.Email, request.Password, cancellationToken);
            return Ok();
        }
        catch(newEXCEPTION)
        {
            return BadRequest("Account")
        }
        
    }
}