using MySuperShop.HttpModels.Requests;

namespace MySuperShop.HttpModels
{
    public interface IMyShopClient
    {
        Task AddProduct(Product product, CancellationToken cancellationToken = default);
        Task<Product> GetProduct(Guid id, CancellationToken cancellationToken = default);
        Task<Product[]> GetProducts(CancellationToken cancellationToken = default);
        Task UpdateProduct(Product product, CancellationToken cancellationToken = default);
        Task DeleteProduct(Product product, CancellationToken cancellationToken = default);
        Task Register(RegisterRequest account, CancellationToken cancellationToken = default);
    }
}