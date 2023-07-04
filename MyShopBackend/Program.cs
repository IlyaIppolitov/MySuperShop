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

var app = builder.Build();

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
async Task<Product> GetProductByIdAsync(AppDbContext dbContext, [FromQuery] Guid guid)
{
    return await dbContext.Products.Where(p => p.Id == guid).FirstAsync();
}

// (C) Add product
async Task AddProductAsync(AppDbContext dbContext, Product product)
{
    await dbContext.Products.AddAsync(product);
    await dbContext.SaveChangesAsync();
}

// (U) Update product
async Task UpdateProductAsync(AppDbContext dbContext, [FromBody] Product product)
{
    await dbContext.Products
        .Where(p => p.Id == product.Id)
        .ExecuteUpdateAsync(s => s
            .SetProperty(p => p.Name, p => product.Name)
            .SetProperty(p => p.Price, p => product.Price)
    );
    await dbContext.SaveChangesAsync();
}

// (U) Update product
async Task UpdateProductByIdAsync(AppDbContext dbContext, [FromQuery] Guid guid, [FromBody] Product product)
{
    await dbContext.Products
        .Where(p => p.Id == guid)
        .ExecuteUpdateAsync(s => s
            .SetProperty(p => p.Id, p => product.Id)
            .SetProperty(p => p.Name, p => product.Name)
            .SetProperty(p => p.Price, p => product.Price)
    );
    await dbContext.SaveChangesAsync();
}

// (D) Delete product
async Task DeleteProductAsync(AppDbContext dbContext, [FromBody] Product product)
{
    await dbContext.Products.Where(p => p.Id == product.Id).ExecuteDeleteAsync();
    await dbContext.SaveChangesAsync();
}
async Task DeleteProductByIdAsync(AppDbContext dbContext, [FromQuery] Guid guid)
{
    await dbContext.Products.Where(p => p.Id == guid).ExecuteDeleteAsync();
    await dbContext.SaveChangesAsync();
}

app.Run();
