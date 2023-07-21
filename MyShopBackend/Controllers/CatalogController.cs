using Microsoft.AspNetCore.Mvc;
using MyShopBackend.Data;

namespace MyShopBackend.Controllers;

public class CatalogController : Controller
{
    private readonly IRepository<Product> _repository;

    public CatalogController(IRepository<Product> repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    [HttpGet("get_products")]
    public async Task<IResult> GetAllProductsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var products =  await _repository.GetAll(cancellationToken);
            return Results.Ok(products);
        }
        catch (Exception e)
        {
            return Results.Empty;
        }
    }
    
    [HttpGet("get_product")]
    public async Task<IResult> GetProductByIdAsync([FromQuery] Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var foundProduct = await _repository.GetById(id, cancellationToken);
            return Results.Ok(foundProduct);
        }
        catch (InvalidOperationException eх)
        {
            return Results.NotFound();
        }
    }
    
    [HttpPost ("add_product")]
    public async Task<IResult> AddProductAsync([FromBody]Product product,CancellationToken cancellationToken)
    {
        try
        {
            await _repository.Add(product, cancellationToken);
            return Results.Created("Created!", product);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.ToString());
        }
    }
    
    [HttpPost("update_product")]
    public async Task<IResult> UpdateProductAsync([FromBody] Product product, CancellationToken cancellationToken)
    {
        try
        {
            await _repository.Update(product, cancellationToken);
            return Results.Ok();
        }
        catch (Exception e)
        {
            return Results.NotFound();
        }
    }
    
    [HttpPost("delete_product")]
    public async Task<IResult> DeleteProductAsync([FromBody] Product product, CancellationToken cancellationToken)
    {
        try
        {
            await _repository.Delete(product, cancellationToken);
            return Results.Ok();
        }
        catch (Exception e)
        {
            return Results.NotFound();
        }
    }
}