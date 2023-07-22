using Microsoft.AspNetCore.Mvc;
using MySuperShop.Domain.Exceptions;
using MySuperShop.Domain.Services;
using MySuperShop.HttpModels.Requests;

namespace MyShopBackend.Controllers;

[Route("account")]
[ApiController]
public class AccountController : Controller
{
    private readonly AccountService _accountService;

    public AccountController(AccountService accountService)
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
        catch (EmailAlreadyExistsException ex)
        {
            return BadRequest($"Account with given email already exists: {ex.Value}");
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest("Argument null exception caught:\n" + ex.Message);
        }
    }
}