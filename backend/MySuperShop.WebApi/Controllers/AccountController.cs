using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopBackend.Services;
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
    private readonly ITokenService _tokenService;

    public AccountController(AccountService accountService, ITokenService tokenService)
    {
        _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
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
            var token = _tokenService.GenerateToken(account);
            return new LoginResponse(account.Id, account.Name, token);
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

    [Authorize]
    [HttpGet("current")]
    public Task<ActionResult<AccountResponse>> GetCurrentAccount(CancellationToken cancellationToken)
    {
        var account = _accountService.GetAccountById(
            Guid.Parse((User.FindFirstValue(ClaimTypes.NameIdentifier))),
            cancellationToken);
        return new AccountResponse(account.Id, account.Name, account.)
    }
}