using MySuperShop.HttpApiClient.Models;
using System.Net.Http.Json;

namespace MySuperShop.HttpApiClient
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

        public async Task<Product[]> GetProducts(CancellationToken cancellationToken = default)
        {
            var products = await _httpClient
                .GetFromJsonAsync<Product[]>($"get_products", cancellationToken);
            if (products is null)
            {
                throw new InvalidOperationException("The server returned null");
            }
            return products;
        }

        public async Task<Product> GetProduct(Guid id, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(id);
            var product = await _httpClient
                .GetFromJsonAsync<Product>($"get_product?id={id}", cancellationToken);
            if (product is null)
            {
                throw new InvalidOperationException("The server returned null");
            }
            return product;
        }

        public async Task AddProduct(Product? product, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(product);
            using var response = await _httpClient
                .PostAsJsonAsync("add_product", product, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task UpdateProduct(Product product, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(product);
            using var response = await _httpClient.PostAsJsonAsync("update_product", product, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteProduct(Product? product, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(product);
            using var response = await _httpClient.PostAsJsonAsync("delete_product", product, cancellationToken);
            response.EnsureSuccessStatusCode();
        }

        public async Task Register(Account account, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(account);
            using var response = await _httpClient.PostAsJsonAsync("account/register", account, cancellationToken);
            response.EnsureSuccessStatusCode();
        }
    }
}
