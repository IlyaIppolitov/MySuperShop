﻿using System.Collections.Concurrent;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using MySuperShop.HttpApiClient.Exceptions;
using MySuperShop.HttpApiClient.Extensions;
using MySuperShop.HttpModels.Requests;
using MySuperShop.HttpModels.Responses;

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

        public async Task<Account[]> GetAccounts(CancellationToken cancellationToken = default)
        {
            var accounts = await _httpClient
                .GetFromJsonAsync<Account[]>($"account/all", cancellationToken);
            if (accounts is null)
            {
                throw new InvalidOperationException("The server returned null");
            }
            return accounts;
        }

        public async Task<UpdateAccountResponse> UpdateAccount(UpdateAccountRequest account, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(account);
            const string uri = "account/update";
            return await _httpClient.PostAsJsonAnsDeserializeAsync<UpdateAccountRequest, UpdateAccountResponse>(account, uri, cancellationToken);
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

        public async Task<RegisterResponse> Register(RegisterRequest account, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(account);

            const string uri = "account/register";

            return await _httpClient.PostAsJsonAnsDeserializeAsync<RegisterRequest, RegisterResponse>(account, uri, cancellationToken);
        }

        public async Task<LoginResponse> Login(LoginRequest request, CancellationToken cancellationToken)
        {
            ArgumentNullException.ThrowIfNull(request);
            const string uri = "account/login";
            var response = await _httpClient.PostAsJsonAnsDeserializeAsync<LoginRequest, LoginResponse>(request, uri, cancellationToken);
            var headerValue = new AuthenticationHeaderValue("Bearer", response.Token);
            _httpClient.DefaultRequestHeaders.Authorization = headerValue;
            return response;
        }

        public async Task<ConcurrentDictionary<string, int>> GetMetrics(CancellationToken cancellationToken = default)
        {
            var metrics = await _httpClient
                .GetFromJsonAsync<ConcurrentDictionary<string, int>>($"metrics", cancellationToken);
            if (metrics is null)
            {
                throw new InvalidOperationException("The server returned null");
            }
            return metrics;
        }
    }
}
