using Microsoft.AspNetCore.Mvc;
using MyShopBackend.Data;
using MySuperShop.Domain.Entities;
using MySuperShop.Domain.Repositories;

namespace MyShopBackend.Controllers;

[ApiController]
public class CatalogController : Controller
{
    private readonly IRepository<Product> _repository;

    public CatalogController(IRepository<Product> repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
    }
    
    [HttpGet("get_products")]
    public async Task<ActionResult<IReadOnlyList<Product>>> GetAllProductsAsync(CancellationToken cancellationToken)
    {
        try
        {
            var products =  await _repository.GetAll(cancellationToken);
            return Ok(products);
        }
        catch (ArgumentNullException e)
        {
            return BadRequest();
        }
    }
    
    [HttpGet("get_product")]
    public async Task<ActionResult<Product>> GetProductByIdAsync([FromQuery] Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var foundProduct = await _repository.GetById(id, cancellationToken);
            return Ok(foundProduct);
        }
        catch (InvalidOperationException eх)
        {
            return NotFound();
        }
    }
    
    [HttpPost ("add_product")]
    public async Task<IActionResult> AddProductAsync([FromBody]Product product,CancellationToken cancellationToken)
    {
        try
        {
            await _repository.Add(product, cancellationToken);
            return Created("Created!", product);
        }
        catch (InvalidOperationException ex)
        {
            return Problem(ex.ToString());
        }
    }
    
    [HttpPost("update_product")]
    public async Task<ActionResult> UpdateProductAsync([FromBody] Product product, CancellationToken cancellationToken)
    {
        try
        {
            await _repository.Update(product, cancellationToken);
            return Ok();
        }
        catch (InvalidOperationException e)
        {
            return NotFound();
        }
    }
    
    [HttpPost("delete_product")]
    public async Task<ActionResult> DeleteProductAsync([FromBody] Product product, CancellationToken cancellationToken)
    {
        try
        {
            await _repository.Delete(product, cancellationToken);
            return Ok();
        }
        catch (InvalidOperationException e)
        {
            return NotFound();
        }
    }
}