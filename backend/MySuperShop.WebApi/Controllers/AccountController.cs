using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using MySuperShop.Domain.Exceptions;
using MySuperShop.Domain.Services;
using MySuperShop.HttpModels.Requests;
using MySuperShop.HttpModels.Responses;
#pragma warning disable CS8604

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
    public async Task<ActionResult<RegisterResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        try
        {
            await _accountService.Register(request.Name, request.Email, request.Password, cancellationToken);
            return new RegisterResponse(request.Name, request.Email);
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

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        LoginRequest reqest,
        CancellationToken cancellationToken)
    {
        try
        {
            var account = await _accountService.Login(reqest.Email, reqest.Password, cancellationToken);
            return new LoginResponse(account.Id, account.Name);
        }
        catch (AccountNotFoundException)
        {
            return Conflict(new ErrorResponse("Аккаунт с таким Email не найден!"));
        }
        catch (InvalidPasswordException)
        {
            return Conflict(new ErrorResponse("Неверный пароль"));
        }
    }
}