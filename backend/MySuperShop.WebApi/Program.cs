using IdentityPasswordHasherLib;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySuperShop.Data.EntityFramework;
using MySuperShop.Data.EntityFramework.Repositories;
using MySuperShop.Domain.Repositories;
using MySuperShop.Domain.Services;

var builder = WebApplication.CreateBuilder(args);

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

// ����������� �����������
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<IAccountRepository, AccountRepositoryEf>();
builder.Services.AddScoped<AccountService>(); // DIP????
builder.Services.AddSingleton<IApplicationPasswordHasher, IdentityPasswordHasher>();

//Логирование всех запросов и ответов
builder.Services.AddHttpLogging(options => //настройка
{
    options.LoggingFields = HttpLoggingFields.RequestHeaders
                            | HttpLoggingFields.ResponseHeaders;
});

var app = builder.Build();

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

app.UseAuthorization();

app.MapControllers();

app.Run();
