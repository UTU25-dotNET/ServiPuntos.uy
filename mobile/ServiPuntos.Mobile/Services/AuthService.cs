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

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "ServiPuntos.Mobile");
        }

        public async Task<bool> LoginWithGoogleAsync()
        {
            try
            {
                var authUrl = $"{API_BASE_URL}/google-login?mobile=true";
                LogInfo($"[AuthService] Abriendo navegador con URL: {authUrl}");
                await Browser.OpenAsync(authUrl, BrowserLaunchMode.SystemPreferred);
                LogInfo("[AuthService] Navegador abierto, esperando callback...");
                return true;
            }
            catch (Exception ex)
            {
                LogInfo($"[AuthService] Error abriendo navegador: {ex.Message}");
                await MainThread.InvokeOnMainThreadAsync(async () =>
                    await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK"));
                return false;
            }
        }

        public async Task<SignInResponse?> SignInAsync(string email, string password)
        {
            LogInfo("[AuthService] === INICIO SIGNIN ===");
            LogInfo($"[AuthService] Email: {email}");
            LogInfo($"[AuthService] API_BASE_URL: {API_BASE_URL}");


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
            LogInfo($"[AuthService] STATUS: {response.StatusCode} - BODY: {body}");

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
            await SecureStorage.SetAsync(USERID_KEY, result.UserId);


            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);

            LogInfo($"[AuthService] Login exitoso para: {result.Email}");


            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                var td = new TokenDisplayPage(this);
                td.SetToken(result.Token);
                LogInfo("[AuthService] Navegando a TokenDisplayPage...");
                await Application.Current.MainPage.Navigation.PushAsync(td);
            });

            return result;
        }

        public async Task SaveTokenAsync(string token)
        {
            try
            {
                await SecureStorage.SetAsync(TOKEN_KEY, token);
                LogInfo("[AuthService] Token guardado exitosamente");
            }
            catch (Exception ex)
            {
                LogInfo($"[AuthService] Error guardando token: {ex.Message}");
            }
        }

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                var t = await SecureStorage.GetAsync(TOKEN_KEY);
                LogInfo($"[AuthService] Token obtenido: {(string.IsNullOrEmpty(t) ? "vacío" : "presente")}");
                return t;
            }
            catch (Exception ex)
            {
                LogInfo($"[AuthService] Error obteniendo token: {ex.Message}");
                return null;
            }
        }

        public async Task<bool> IsAuthenticatedAsync() =>
            !string.IsNullOrEmpty(await GetTokenAsync());

        public async Task LogoutAsync()
        {
            try
            {
                SecureStorage.Remove(TOKEN_KEY);
                SecureStorage.Remove(USERID_KEY);
                _httpClient.DefaultRequestHeaders.Authorization = null;
                LogInfo("[AuthService] Logout completado");
            }
            catch (Exception ex)
            {
                LogInfo($"[AuthService] Error en logout: {ex.Message}");
            }
        }

        public async Task<UserInfo?> GetUserInfoAsync()
        {
            try
            {
                var resp = await _httpClient.GetAsync($"{API_BASE_URL}/userinfo");
                if (!resp.IsSuccessStatusCode) return null;
                var json = await resp.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<UserInfo>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch
            {
                return null;
            }
        }

        public async Task<List<TenantResponse>> GetTenantsAsync()
        {
            try
            {
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                      HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                using var client = new HttpClient(handler);
                var resp = await client.GetAsync($"{API_BASE_URL}/tenants");
                if (!resp.IsSuccessStatusCode) return new();
                var json = await resp.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<TenantResponse>>(json,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new();
            }
            catch
            {
                return new();
            }
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            try
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
                var body = await resp.Content.ReadAsStringAsync();
                LogInfo($"[AuthService] Register Status: {resp.StatusCode} Body: {body}");
                return resp.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }

        public void Dispose() =>
            _httpClient?.Dispose();
    }
}
