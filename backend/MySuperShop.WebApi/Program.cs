using System.Collections.Concurrent;
using IdentityPasswordHasherLib;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MyShopBackend.Configurations;
using MyShopBackend.Middleware;
using MyShopBackend.Services;
using MySuperShop.Data.EntityFramework;
using MySuperShop.Data.EntityFramework.Repositories;
using MySuperShop.Data.EntityFramework.Service;
using MySuperShop.Domain.Repositories;
using MySuperShop.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

JwtConfig jwtConfig = builder.Configuration
    .GetRequiredSection("JwtConfig")
    .Get<JwtConfig>()!;
if (jwtConfig is null) throw new InvalidOperationException("JwtConfig is not configured!");
builder.Services.AddSingleton(jwtConfig);


// Add services to the container.

// ��������� ����������� - ���� ������
var dbPath = "myapp.db";
builder.Services.AddDbContext<AppDbContext>(
   options => options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ���������� CORS � builder
builder.Services.AddCors();

// Подключение smtpConfig, настраемого из файла конфигураций json
builder.Services.AddOptions<SmtpConfig>()
    .BindConfiguration("SmtpConfig")
    .ValidateDataAnnotations()
    .ValidateOnStart();

// ����������� �����������
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<IAccountRepository, AccountRepositoryEf>();
builder.Services.AddScoped<ICartRepository, CartRepositoryEf>();
builder.Services.AddScoped<AccountService>(); // DIP????
builder.Services.AddScoped<CartService>(); // DIP????
builder.Services.AddSingleton<ITransitionCounterService, TransitionCounterService>();
builder.Services.AddSingleton<IApplicationPasswordHasher, IdentityPasswordHasher>();
builder.Services.AddSingleton<ITokenService, TokenService>();
builder.Services.AddScoped<IConfirmationCodeRepository, ConfirmationCodeRepositoryEf>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWorkEf>();
builder.Services.AddScoped<IEmailSender, MailKitSmtpEmailSender>(); 

//Логирование всех запросов и ответов
builder.Services.AddHttpLogging(options => //настройка
{
    options.LoggingFields = HttpLoggingFields.RequestHeaders
                            | HttpLoggingFields.ResponseHeaders;
});


builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(jwtConfig.SigningKeyBytes),
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            RequireExpirationTime = true,
            RequireSignedTokens = true,
          
            ValidateAudience = true,
            ValidateIssuer = true,
            ValidAudiences = new[] { jwtConfig.Audience },
            ValidIssuer = jwtConfig.Issuer
        };
    });
builder.Services.AddAuthorization();



var app = builder.Build();

app.UseMiddleware<TransitionsCounterMiddleware>();

// app.Use(async (context, next) =>
// {
//     IHeaderDictionary headers = context.Request.Headers;
//     if (!headers["User-Agent"].ToString().Contains("Edg"))
//     {
//         await context.Response.WriteAsync(
//             "We support only Edge browser!");
//         return; //прерываем выполнение конвейера
//     }
//
//     await next();
// });

// Подключение логирования через AddHttpLogging (в building)
// app.UseHttpLogging();

// ���������� CORS � WebApplication
app.UseCors(policy =>
{
    policy
        .WithOrigins("https://localhost:5001")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin();
});


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();


app.MapControllers();

app.MapGet("/metrics", async (
    HttpContext context, 
    ITransitionCounterService counterService) =>
{
        var counter = counterService.GetCounter();
        await context.Response.WriteAsJsonAsync(counter);
});

app.Run();
