using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyShopBackend.Services;
using MySuperShop.Domain.Entities;
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
            var customerRole = new Role[] { Role.Customer };
            await _accountService.Register(request.Name, request.Email, request.Password, customerRole, cancellationToken);
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
    public async Task<ActionResult<AccountResponse>> GetCurrentAccount(CancellationToken cancellationToken)
    {
        var strId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var guid = Guid.Parse(strId);
        var account = await _accountService.GetAccountById(guid, cancellationToken);
        return new AccountResponse(account.Id, account.Name, account.Email, account.Roles.ToString());
    }

    [HttpPost("update")]
    public async Task<ActionResult<UpdateAccountResponse>> UpdateAccount(UpdateAccountRequest request, CancellationToken cancellationToken)
    {
        var account = await _accountService.UpdateAccount(request.Id, request.Name, request.Email, request.Password, request.Roles, cancellationToken);
        
        return new UpdateAccountResponse(account.Id, account.Name, account.Email);
    }
    
    // [Authorize(Roles = nameof(Role.Admin))]
    [HttpGet("all")]
    public async Task<ActionResult<Account[]>> GetAllAccounts(CancellationToken cancellationToken)
    {
        try
        {
            var products =  await _accountService.GetAll(cancellationToken);
            return Ok(products);
        }
        catch (ArgumentNullException e)
        {
            return Conflict(new ErrorResponse("Аккаунты отсутствуют!"));
        }
    }
}