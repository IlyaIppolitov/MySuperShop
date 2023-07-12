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

var app = builder.Build();

// Добавление CORS в WebApplication
app.UseCors(policy =>
{
    policy
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
app.MapPost("/update_product_by_id", UpdateProductByIdAsync);
app.MapPost("/delete_product", DeleteProductAsync);
app.MapPost("/delete_product_by_id", DeleteProductByIdAsync);


// (R) Read All
async Task<Product[]> GetAllProductsAsync(AppDbContext dbContext)
{
    return await dbContext.Products.ToArrayAsync();
}

// (R) Read product by Id
async Task<IResult> GetProductByIdAsync(AppDbContext dbContext, [FromQuery] Guid id)
{
    var foundProduct = await dbContext.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
    if (foundProduct == null)
    {
        return Results.NotFound($"Product with Id {id} not found");
    }
    return Results.Ok(foundProduct);
}

// (C) Add product
async Task<IResult> AddProductAsync(AppDbContext dbContext, Product product)
{
    await dbContext.Products.AddAsync(product);
    await dbContext.SaveChangesAsync();

    return Results.Created("Объект создан", product);
}

// (U) Update product
async Task<IResult> UpdateProductAsync(AppDbContext dbContext, Product product)
{
    var foundProduct = await dbContext.Products.Where(p => p.Id == product.Id).FirstOrDefaultAsync();
    if (foundProduct == null)
    {
        return Results.NotFound($"Product with Id {product.Id} not found");
    }

    foundProduct.Name = product.Name;
    foundProduct.Price = product.Price;
    await dbContext.SaveChangesAsync();

    return Results.Ok();
}

// (U) Update product by Id
async Task<IResult> UpdateProductByIdAsync(AppDbContext dbContext, [FromQuery] Guid id, [FromBody] Product product)
{
    var foundProduct = await dbContext.Products.Where(p => p.Id == id).FirstOrDefaultAsync();
    if (foundProduct == null)
    {
        return Results.NotFound($"Product with Id {id} not found");
    }

    foundProduct.Name = product.Name;
    foundProduct.Price = product.Price;
    await dbContext.SaveChangesAsync();

    return Results.Ok();
}

// (D) Delete product
async Task<IResult> DeleteProductAsync(AppDbContext dbContext, [FromBody] Product product)
{
    var deletedCount = await dbContext.Products.Where(p => p.Id == product.Id).ExecuteDeleteAsync();

    if (deletedCount == 0)
    {
        return Results.NotFound($"Product with Id {product.Id} not found");
    }
    await dbContext.SaveChangesAsync();
    return Results.Ok();
}
// (D) Delete product by Id
async Task<IResult> DeleteProductByIdAsync(AppDbContext dbContext, [FromQuery] Guid id)
{
    var deletedCount = await dbContext.Products.Where(p => p.Id == id).ExecuteDeleteAsync();

    if (deletedCount == 0)
    {
        return Results.NotFound($"Product with Id {id} not found");
    }
    await dbContext.SaveChangesAsync();
    return Results.Ok();
}

app.Run();
