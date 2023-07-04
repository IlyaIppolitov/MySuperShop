using MySuperShop.Models;

namespace MySuperShop.Interfaces
{
    public interface ICatalog
    {
        /// <summary>
        /// Получить коллекцию товаров
        /// </summary>
        /// <returns>Коллекция товаров</returns>
        List<Product> GetProducts(ICurrentTime curTime);
        public Task<List<Product>> GetProductsAsync(ICurrentTime curTime);

        /// <summary>
        /// Получить товар по значению ID
        /// </summary>
        /// <param name="productId"> ID </param>
        /// <returns>Товар</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        Product GetProductById(Guid productId, ICurrentTime curTime);

        /// <summary>
        /// Добавить товар
        /// </summary>
        /// <param name="product">Товар</param>
        void AddProduct(Product product);

        /// <summary>
        /// Удалить товар
        /// </summary>
        /// <param name="productId">Id товара</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        void DeleteProduct(Guid productId);

        /// <summary>
        /// Обновить информацию о товаре
        /// </summary>
        /// <param name="product">Товар</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        void UpdateProduct(Product newProduct);

        /// <summary>
        /// Обновить информацию о товаре по его ID
        /// </summary>
        /// <param name="product">Id товара</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        void UpdateProductById(Guid productId, Product newProduct);

        /// <summary>
        /// Очистить каталог
        /// </summary>
        void Clear();
    }
}
