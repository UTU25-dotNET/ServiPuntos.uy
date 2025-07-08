using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Helpers;
using static ServiPuntos.Mobile.Services.AppLogger;

namespace ServiPuntos.Mobile.Services
{
    public class AuthService : IAuthService, IDisposable
    {
        private readonly HttpClient _httpClient;
        private bool _isLoggedIn;
        private const string TOKEN_KEY = "auth_token";
        private const string USERID_KEY = "userId";
        private const string TENANT_KEY = "tenant_id";
        private const string TENANT_COLOR_KEY = "tenant_color";

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;

            // Al iniciar, cargamos token existente (si hay) y configuramos header
            try
            {
                var stored = SecureStorage.GetAsync(TOKEN_KEY).GetAwaiter().GetResult();
                if (!string.IsNullOrEmpty(stored))
                {
                    _isLoggedIn = true;
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", stored);
                }
            }
            catch
            {
                _isLoggedIn = false;
            }
        }

        public bool IsLoggedIn => _isLoggedIn;

        public async Task<bool> LoginWithGoogleAsync()
        {
            var loginUrl = new Uri(_httpClient.BaseAddress!, "google-login?mobile=true").ToString();
            await Browser.OpenAsync(loginUrl, BrowserLaunchMode.SystemPreferred);
            LogInfo($"[AuthService] Opened Google login URL: {loginUrl}");
            return true;
        }

        public async Task<SignInResponse?> SignInAsync(string email, string password)
        {
            LogInfo($"[AuthService] Signing in user: {email}");
            var payload = JsonSerializer.Serialize(new { email, password });
            using var content = new StringContent(payload, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("signin", content);
            var body = await response.Content.ReadAsStringAsync();
            LogInfo($"[AuthService] Response status: {response.StatusCode} Body: {body}");

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Error de autenticación: {response.StatusCode}");

            var root = JsonDocument.Parse(body).RootElement;

            // Guardar color de tenant, si viene
            if (root.TryGetProperty("color", out var colorProp)
             || root.TryGetProperty("tenantColor", out colorProp))
            {
                var tenantColor = colorProp.GetString();
                if (!string.IsNullOrEmpty(tenantColor))
                {
                    await SecureStorage.SetAsync(TENANT_COLOR_KEY, tenantColor);
                    LogInfo($"[AuthService] TenantColor saved: {tenantColor}");
                }
            }

            var result = new SignInResponse
            {
                Token = root.GetProperty("token").GetString()!,
                UserId = root.GetProperty("userId").GetString()!,
                Username = root.GetProperty("username").GetString()!,
                Email = root.GetProperty("email").GetString()!,
                Role = root.GetProperty("role").GetInt32().ToString(),
                TenantId = root.GetProperty("tenantId").GetString()!
            };

            // Almacenamos token y datos
            await SecureStorage.SetAsync(TOKEN_KEY, result.Token);
            await SecureStorage.SetAsync(USERID_KEY, result.UserId);
            await SecureStorage.SetAsync(TENANT_KEY, result.TenantId);
            LogInfo($"[AuthService] Token and IDs saved");

            // Configuramos header y estado interno
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", result.Token);
            _isLoggedIn = true;

            // Notificamos a la Shell que el usuario ha iniciado sesión
            MessagingCenter.Send(this, MessagingConstants.UserLoggedIn);

            return result;
        }

        public async Task SaveTokenAsync(string token)
        {
            await SecureStorage.SetAsync(TOKEN_KEY, token);
            LogInfo($"[AuthService] SaveTokenAsync: {token}");
        }

        public async Task<string?> GetTokenAsync()
        {
            var t = await SecureStorage.GetAsync(TOKEN_KEY);
            LogInfo($"[AuthService] GetTokenAsync: {(string.IsNullOrEmpty(t) ? "null" : t)}");
            return t;
        }

        public async Task<UserInfo?> GetUserInfoAsync()
        {
            var resp = await _httpClient.GetAsync("userinfo");
            LogInfo($"[AuthService] GetUserInfoAsync status: {resp.StatusCode}");
            if (!resp.IsSuccessStatusCode)
                return null;

            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserInfo>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }

        public async Task<List<TenantResponse>> GetTenantsAsync()
        {
            var resp = await _httpClient.GetAsync("tenants");
            LogInfo($"[AuthService] GetTenantsAsync status: {resp.StatusCode}");
            if (!resp.IsSuccessStatusCode)
                return new List<TenantResponse>();

            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<TenantResponse>>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            ) ?? new List<TenantResponse>();
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            var json = JsonSerializer.Serialize(request);
            using var c = new StringContent(json, Encoding.UTF8, "application/json");
            var resp = await _httpClient.PostAsync("register", c);
            LogInfo($"[AuthService] RegisterAsync status: {resp.StatusCode}");
            return resp.IsSuccessStatusCode;
        }

        public async Task LogoutAsync()
        {
            // Borramos credenciales
            SecureStorage.Remove(TOKEN_KEY);
            SecureStorage.Remove(USERID_KEY);
            SecureStorage.Remove(TENANT_KEY);
            SecureStorage.Remove(TENANT_COLOR_KEY);

            _httpClient.DefaultRequestHeaders.Authorization = null;
            _isLoggedIn = false;
            LogInfo("[AuthService] LogoutAsync completed");

            // Notificamos a la Shell que el usuario cerró sesión
            MessagingCenter.Send(this, MessagingConstants.UserLoggedOut);

            await Task.CompletedTask;
        }

        public void Dispose() =>
            _httpClient.Dispose();
    }
}
