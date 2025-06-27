using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Views;
using static ServiPuntos.Mobile.Services.AppLogger;

namespace ServiPuntos.Mobile.Services
{
    public class AuthService : IAuthService, IDisposable
    {
        private readonly HttpClient _httpClient;
        private const string API_BASE_URL = "https://ec2-18-220-251-96.us-east-2.compute.amazonaws.com:5019/api/auth";
        private const string TOKEN_KEY = "auth_token";
        private const string USERID_KEY = "userId";
        private const string TENANT_KEY = "tenant_id";

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "ServiPuntos.Mobile");
        }

        public async Task<bool> LoginWithGoogleAsync()
        {
            var authUrl = $"{API_BASE_URL}/google-login?mobile=true";
            await Browser.OpenAsync(authUrl, BrowserLaunchMode.SystemPreferred);
            Console.WriteLine($"[AuthService] Opened Google login URL: {authUrl}");
            return true;
        }

        public async Task<SignInResponse?> SignInAsync(string email, string password)
        {
            Console.WriteLine($"[AuthService] Signing in user: {email}");
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            using var client = new HttpClient(handler);
            var payload = JsonSerializer.Serialize(new { email, password });
            var content = new StringContent(payload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync($"{API_BASE_URL}/signin", content);
            var body = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"[AuthService] Response status: {response.StatusCode} Body: {body}");
            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Error de autenticación: {response.StatusCode}");
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;
            var result = new SignInResponse
            {
                Token = root.GetProperty("token").GetString()!,
                UserId = root.GetProperty("userId").GetString()!,
                Username = root.GetProperty("username").GetString()!,
                Email = root.GetProperty("email").GetString()!,
                Role = root.GetProperty("role").GetInt32().ToString(),
                TenantId = root.GetProperty("tenantId").GetString()!
            };
            await SecureStorage.SetAsync(TOKEN_KEY, result.Token);
            Console.WriteLine($"[AuthService] Token saved: {result.Token}");
            await SecureStorage.SetAsync(USERID_KEY, result.UserId);
            Console.WriteLine($"[AuthService] UserId saved: {result.UserId}");
            await SecureStorage.SetAsync(TENANT_KEY, result.TenantId);
            Console.WriteLine($"[AuthService] TenantId saved: {result.TenantId}");
            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);
            var td = new TokenDisplayPage(this);
            td.SetToken(result.Token);
            await Application.Current.MainPage.Navigation.PushAsync(td);
            return result;
        }

        public async Task SaveTokenAsync(string token)
        {
            await SecureStorage.SetAsync(TOKEN_KEY, token);
            Console.WriteLine($"[AuthService] SaveTokenAsync: {token}");
        }

        public async Task<string?> GetTokenAsync()
        {
            var t = await SecureStorage.GetAsync(TOKEN_KEY);
            Console.WriteLine($"[AuthService] GetTokenAsync: {(string.IsNullOrEmpty(t) ? "null" : t)}");
            return t;
        }

        public async Task<bool> IsAuthenticatedAsync() =>
            !string.IsNullOrEmpty(await GetTokenAsync());

        public async Task LogoutAsync()
        {
            SecureStorage.Remove(TOKEN_KEY);
            SecureStorage.Remove(USERID_KEY);
            SecureStorage.Remove(TENANT_KEY);
            _httpClient.DefaultRequestHeaders.Authorization = null;
            Console.WriteLine("[AuthService] LogoutAsync completed");
        }

        public async Task<UserInfo?> GetUserInfoAsync()
        {
            var resp = await _httpClient.GetAsync($"{API_BASE_URL}/userinfo");
            Console.WriteLine($"[AuthService] GetUserInfoAsync status: {resp.StatusCode}");
            if (!resp.IsSuccessStatusCode) return null;
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserInfo>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<List<TenantResponse>> GetTenantsAsync()
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            using var client = new HttpClient(handler);
            var resp = await client.GetAsync($"{API_BASE_URL}/tenants");
            Console.WriteLine($"[AuthService] GetTenantsAsync status: {resp.StatusCode}");
            if (!resp.IsSuccessStatusCode) return new();
            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<TenantResponse>>(json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            var handler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback =
                    HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
            };
            using var client = new HttpClient(handler);
            var json = JsonSerializer.Serialize(request);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await client.PostAsync($"{API_BASE_URL}/register", content);
            Console.WriteLine($"[AuthService] RegisterAsync status: {resp.StatusCode}");
            return resp.IsSuccessStatusCode;
        }

        public void Dispose() =>
            _httpClient.Dispose();
    }
}
