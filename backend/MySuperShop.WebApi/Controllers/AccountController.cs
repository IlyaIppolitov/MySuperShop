﻿using System.Security.Claims;
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
public class AccountController : ControllerBase
{
    private readonly AccountService _accountService;
    private readonly ITokenService _tokenService;

    public AccountController(AccountService accountService, ITokenService tokenService)
    {
        _accountService = accountService ?? throw new ArgumentNullException(nameof(accountService));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
    }
    
    [HttpPost("register")]
    public async Task<ActionResult<LoginByCodeResponse>> Register(RegisterRequest request, CancellationToken cancellationToken)
    {
        if (request == null) throw new ArgumentNullException(nameof(request));
        try
        {
            var customerRole = new Role[] { Role.Customer };
            await _accountService.Register(request.Name, request.Email, request.Password, customerRole, cancellationToken);
            var (account, codeId) = await _accountService.Login(request.Email, request.Password, cancellationToken);
            var token = _tokenService.GenerateToken(account);
            return new LoginByCodeResponse(account.Id, account.Name, token);
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

    // 1-st stage of login
    [HttpPost("login")]
    public async Task<ActionResult<LoginByCodeResponse>> Login(
        LoginByPassRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var (account, codeId) = await _accountService.Login(request.Email, request.Password, cancellationToken);
            var token = _tokenService.GenerateToken(account);
            return new LoginByCodeResponse(account.Id, account.Name, token);
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
    
    // 2-nd stage of login
    [HttpPost("login_by_code")]
    public async Task<ActionResult<LoginByCodeResponse>> LoginByCode(
        LoginByCodeRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var account = await _accountService.LoginByCode(request.Email, request.CodeId, request.Code, cancellationToken);
            var token = _tokenService.GenerateToken(account);
            return new LoginByCodeResponse(account.Id, account.Name, token);
        }
        catch (AccountNotFoundException)
        {
            return Conflict(new ErrorResponse("Аккаунт с таким Email не найден!"));
        }
        catch (CodeNotFoundException)
        {
            return Conflict(new ErrorResponse("Код не генерировался для данного аккаунта!"));
        }
        catch (InvalidCodeException)
        {
            return Conflict(new ErrorResponse("Код плохой!"));
        }

    }
    



    [Authorize]
    [HttpGet("current")]
    public async Task<ActionResult<AccountResponse>> GetCurrentAccount(CancellationToken cancellationToken)
    {
        try
        {
            var strId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var guid = Guid.Parse(strId);
            var account = await _accountService.GetAccountById(guid, cancellationToken);
            var resp = new AccountResponse(account.Id, account.Name, account.Email, string.Join(",", account.Roles));
            return resp;
        }
        catch (AccountNotFoundException)
        {
            return Conflict(new ErrorResponse("Аккаунт с таким Email не найден!"));
        }
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