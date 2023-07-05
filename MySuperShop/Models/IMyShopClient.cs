namespace MySuperShop.Models
{
    public interface IMyShopClient
    {
        Task AddProduct(Product product);
        Task<Product> GetProduct(Guid id);
    }
}