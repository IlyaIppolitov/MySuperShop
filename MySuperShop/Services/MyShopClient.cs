using MySuperShop.Interfaces;
using MySuperShop.Models;
using System.Net.Http.Json;

namespace MySuperShop.Services
{

    public class MyShopClient : IDisposable, IMyShopClient
    {
        private readonly string _host;
        private readonly HttpClient _httpClient;
        private bool _httpClientInjected = false;

        public MyShopClient(string host = "https://myshop.com", HttpClient? httpClient = null)
        {
            if (string.IsNullOrEmpty(host))
                throw new ArgumentNullException(nameof(host));

            if (Uri.TryCreate(String.Format("https://{0}", host), UriKind.Absolute, out var hostUri))
            // {
            //     throw new ArgumentException("The host address should be a valid url", nameof(host));
            // }
            this._host = host;
            if (httpClient is null)
            {
                this._httpClient = new HttpClient();
            } 
            else 
            {
                this._httpClient = httpClient;
                this._httpClientInjected = true;
            }
            if (_httpClient.BaseAddress is null)
            {
                _httpClient.BaseAddress = hostUri;
            }
        }

        public void Dispose()
        {
            if (!_httpClientInjected)
                ((IDisposable)_httpClient).Dispose();
        }

        public async Task<Product[]> GetProducts()
        {
            var products = await _httpClient.GetFromJsonAsync<Product[]>($"get_products");
            if (products is null)
            {
                throw new InvalidOperationException("The server returned null");
            }
            return products;
        }

        public async Task<Product> GetProduct(Guid id)
        {
            ArgumentNullException.ThrowIfNull(id);
            var product = await _httpClient.GetFromJsonAsync<Product>($"get_product?id={id}");
            if (product is null)
            {
                throw new InvalidOperationException("The server returned null");
            }
            return product;
        }

        public async Task AddProduct(Product product)
        {
            ArgumentNullException.ThrowIfNull(product);
            using var response = await _httpClient.PostAsJsonAsync("add_product", product);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateProduct(Product product)
        {
            ArgumentNullException.ThrowIfNull(product);
            using var response = await _httpClient.PostAsJsonAsync("update_product", product);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteProduct(Product product)
        {
            ArgumentNullException.ThrowIfNull(product);
            using var response = await _httpClient.PostAsJsonAsync("delete_product", product);
            response.EnsureSuccessStatusCode();
        }
    }
}
