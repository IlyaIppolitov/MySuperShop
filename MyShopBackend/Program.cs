using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyShopBackend.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// Добавляем зависимость - база данных
var dbPath = "myapp.db";
builder.Services.AddDbContext<AppDbContext>(
   options => options.UseSqlite($"Data Source={dbPath}"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Добавление CORS в builder
builder.Services.AddCors();

// Подключение репозитория базы данных
builder.Services.AddScoped<IProductRepository, ProductRepositoryEf>();

var app = builder.Build();

// Добавление CORS в WebApplication
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

app.MapGet("/get_products", GetAllProductsAsync);
app.MapGet("/get_product", GetProductByIdAsync);
app.MapPost("/add_product", AddProductAsync);
app.MapPost("/update_product", UpdateProductAsync);
app.MapPost("/delete_product", DeleteProductAsync);


// (R) Read All
async Task<IResult> GetAllProductsAsync(IProductRepository repository, CancellationToken cancellationToken)
{
    try
    {
        var products =  await repository.GetProducts(cancellationToken);
        return Results.Ok(products);
    }
    catch (Exception e)
    {
        return Results.Empty;
    }
}

// (R) Read product by Id
async Task<IResult> GetProductByIdAsync(
    IProductRepository repository,
    [FromQuery] Guid id,
    CancellationToken cancellationToken)
{
    try
    {
        var foundProduct = await repository.GetProductById(id, cancellationToken);
        return Results.Ok(foundProduct);
    }
    catch (InvalidOperationException eх)
    {
        return Results.NotFound();
    }
}

// (C) Add product
async Task<IResult> AddProductAsync(
    IProductRepository repository,
    Product product, 
    CancellationToken cancellationToken)
{
    try
    {
        await repository.Add(product, cancellationToken);
        return Results.Created("Объект создан!", product);
    }
    catch (NotSupportedException ex)
    {
        return Results.Problem(ex.ToString());
    }
}

// (U) Update product
async Task<IResult> UpdateProductAsync(
    IProductRepository repository,
    [FromBody] Product product,
    CancellationToken cancellationToken)
{
    try
    {
        await repository.Update(product, cancellationToken);
        return Results.Ok();
    }
    catch (Exception e)
    {
        return Results.NotFound();
    }
}

// (D) Delete product
async Task<IResult> DeleteProductAsync(
    IProductRepository repository,
    [FromBody] Product product,
    CancellationToken cancellationToken)
{
    try
    {
        await repository.Delete(product, cancellationToken);
        return Results.Ok();
    }
    catch (Exception e)
    {
        return Results.NotFound();
    }
}

app.Run();
