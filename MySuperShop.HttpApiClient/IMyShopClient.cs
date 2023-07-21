using MySuperShop.HttpApiClient.Models;

namespace MySuperShop.HttpApiClient
{
    public interface IMyShopClient
    {
        Task AddProduct(Product product, CancellationToken cancellationToken = default);
        Task<Product> GetProduct(Guid id, CancellationToken cancellationToken = default);
        Task<Product[]> GetProducts(CancellationToken cancellationToken = default);
        Task UpdateProduct(Product product, CancellationToken cancellationToken = default);
        Task DeleteProduct(Product product, CancellationToken cancellationToken = default);
        Task Register(Account account, CancellationToken cancellationToken = default);
    }
}