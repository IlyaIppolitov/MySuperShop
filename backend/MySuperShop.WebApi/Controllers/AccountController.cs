using Microsoft.AspNetCore.Mvc;
using MySuperShop.Domain.Exceptions;
using MySuperShop.Domain.Services;
using MySuperShop.HttpModels.Requests;
using MySuperShop.HttpModels.Responses;

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
            return Conflict( new ErrorResponse("Аккаунт с таким Email уже зарегистрирован!"));
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest("Argument null exception caught:\n" + ex.Message);
        }
    }
}