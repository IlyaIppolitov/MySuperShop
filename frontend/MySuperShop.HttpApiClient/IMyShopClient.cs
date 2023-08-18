using System.Collections.Concurrent;
using MySuperShop.HttpModels.Requests;
using MySuperShop.HttpModels.Responses;

namespace MySuperShop.HttpApiClient
{
    public interface IMyShopClient
    {
        Task AddProduct(Product product, CancellationToken cancellationToken = default);
        Task<Product> GetProduct(Guid id, CancellationToken cancellationToken = default);
        Task<Product[]> GetProducts(CancellationToken cancellationToken = default);
        Task UpdateProduct(Product product, CancellationToken cancellationToken = default);
        Task DeleteProduct(Product product, CancellationToken cancellationToken = default);
        Task<RegisterResponse> Register(RegisterRequest account, CancellationToken cancellationToken = default);
        Task<LoginResponse> Login(LoginRequest request, CancellationToken cancellationToken = default);
        Task<ConcurrentDictionary<string, int>> GetMetrics(CancellationToken cancellationToken = default);
    }
}