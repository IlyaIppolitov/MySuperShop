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

var app = builder.Build();

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

// app.MapGet("/get_products", GetAllProductsAsync);
// app.MapGet("/get_product", GetProductByIdAsync);
// app.MapPost("/add_product",  AddProductAsync);
// app.MapPost("/update_product", UpdateProductAsync);
// app.MapPost("/delete_product", DeleteProductAsync);

// app.MapGet("/", (IAccountRepository repo) => { });


// // (R) Read All
// async Task<IResult> GetAllProductsAsync(IRepository<Product> repository, CancellationToken cancellationToken)
// {
//     try
//     {
//         var products =  await repository.GetAll(cancellationToken);
//         return Results.Ok(products);
//     }
//     catch (Exception e)
//     {
//         return Results.Empty;
//     }
// }
//
// // (R) Read product by Id
// async Task<IResult> GetProductByIdAsync(
//     IRepository<Product> repository,
//     [FromQuery] Guid id,
//     CancellationToken cancellationToken)
// {
//     try
//     {
//         var foundProduct = await repository.GetById(id, cancellationToken);
//         return Results.Ok(foundProduct);
//     }
//     catch (InvalidOperationException e�)
//     {
//         return Results.NotFound();
//     }
// }
//
// // (C) Add product
// async Task<IResult> AddProductAsync(
//     IRepository<Product> repository,
//     Product product, 
//     CancellationToken cancellationToken)
// {
//     try
//     {
//         await repository.Add(product, cancellationToken);
//         return Results.Created("Created!", product);
//     }
//     catch (Exception ex)
//     {
//         return Results.Problem(ex.ToString());
//     }
// }
//
// // (U) Update product
// async Task<IResult> UpdateProductAsync(
//     IRepository<Product> repository,
//     [FromBody] Product product,
//     CancellationToken cancellationToken)
// {
//     try
//     {
//         await repository.Update(product, cancellationToken);
//         return Results.Ok();
//     }
//     catch (Exception e)
//     {
//         return Results.NotFound();
//     }
// }
//
// // (D) Delete product
// async Task<IResult> DeleteProductAsync(
//     IRepository<Product> repository,
//     [FromBody] Product product,
//     CancellationToken cancellationToken)
// {
//     try
//     {
//         await repository.Delete(product, cancellationToken);
//         return Results.Ok();
//     }
//     catch (Exception e)
//     {
//         return Results.NotFound();
//     }
// }

app.Run();
