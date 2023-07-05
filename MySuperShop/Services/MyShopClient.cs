using MySuperShop.Models;
using System.Net.Http.Json;

namespace MySuperShop.Services
{
    public class MyShopClient : IDisposable, IMyShopClient
    {
        private readonly string _host;
        private readonly HttpClient _httpClient;
        private bool _httpClientInjected = false;

        public MyShopClient(string host = "http://myshop.com", HttpClient? httpClient = null)
        {
            ArgumentException.ThrowIfNullOrEmpty(host);
            if (Uri.TryCreate(host, UriKind.Absolute, out var hostUri))
            {
                throw new ArgumentException("The host address should be a valid url", nameof(host));
            }
            this._host = host;
            if (httpClient == null)
            {

            }
            this._httpClient = httpClient ?? new HttpClient();
            if (_httpClient.BaseAddress is null)
            {
                _httpClient.BaseAddress = hostUri;
            }
        }

        public void Dispose()
        {
            ((IDisposable)_httpClient).Dispose();
        }

        public async Task<Product> GetProduct(Guid id)
        {
            var product = await _httpClient.GetFromJsonAsync<Product>($"get_product?id={id}");
            if (product == null)
            {
                throw new InvalidOperationException("The server returned null");
            }
            return product;
        }

        public async Task AddProduct(Product product)
        {

        }
    }
}
