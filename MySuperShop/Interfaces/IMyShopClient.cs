using MySuperShop.Models;

namespace MySuperShop.Interfaces
{
    public interface IMyShopClient
    {
        Task AddProduct(Product product);
        Task<Product> GetProduct(Guid id);
        Task<Product[]> GetProducts();
        Task UpdateProduct(Product product);
        Task DeleteProduct(Product product);
    }
}