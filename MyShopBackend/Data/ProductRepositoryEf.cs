﻿using Microsoft.EntityFrameworkCore;

namespace MyShopBackend.Data;

public class ProductRepositoryEf : IProductRepository
{
    private readonly AppDbContext _dbContext;
    
    public ProductRepositoryEf(AppDbContext dbContext)
    {
        this._dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
    }
    
    public async Task Add(Product product, CancellationToken cancellationToken = default)
    {
        if (product is null)
            throw new ArgumentNullException(nameof(product));
        await _dbContext.Products.AddAsync(product, cancellationToken);
        _dbContext.SaveChanges();
    }

    public async Task Update(Product product, CancellationToken cancellationToken = default)
    {
        if (product is null)
            throw new ArgumentNullException(nameof(product));
        _dbContext.Products.Update(product);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task Delete(Product product, CancellationToken cancellationToken = default)
    {
        if (product is null)
            throw new ArgumentNullException(nameof(product));
        
        _dbContext.Products.Remove(product);
        
        return _dbContext.SaveChangesAsync(cancellationToken);
    }

    public Task<Product> GetProductById(Guid id, CancellationToken cancellationToken = default)
    {
        return _dbContext.Products.FirstAsync(product => product.Id == id, cancellationToken);
    }

    public async Task<Product[]> GetProducts(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Products.ToArrayAsync(cancellationToken);
    }
}