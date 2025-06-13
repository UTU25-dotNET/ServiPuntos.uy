using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.Models;
using static ServiPuntos.Mobile.Services.AppLogger;
using System.Net.Http.Json;

namespace ServiPuntos.Mobile.Services
{
    public interface IAuthService
    {
        Task<bool> LoginWithGoogleAsync();
        Task<SignInResponse?> SignInAsync(string email, string password);
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetTokenAsync();
        Task LogoutAsync();
        Task SaveTokenAsync(string token);
        Task<UserInfo?> GetUserInfoAsync();
        Task<bool> RegisterAsync(RegisterRequest request);
        Task<bool> RefreshTokenAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private const string TOKEN_KEY = "auth_token";
        private const string USERID_KEY = "user_id";

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> LoginWithGoogleAsync()
        {
            try
            {
                var authUrl = new Uri(_httpClient.BaseAddress, "google-login?mobile=true").ToString();
                LogInfo($"[AuthService] Abrir navegador: {authUrl}");
                await Browser.OpenAsync(authUrl, BrowserLaunchMode.SystemPreferred);
                return true;
            }
            catch (Exception ex)
            {
                LogError($"[AuthService] Error navegador: {ex}");
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK")
                );
                return false;
            }
        }

        public async Task<SignInResponse?> SignInAsync(string email, string password)
        {
            LogInfo($"[AuthService] POST signin");
            var req = new SignInRequest { Email = email, Password = password };
            var payload = JsonSerializer.Serialize(req);

            HttpResponseMessage response;
            try
            {
                response = await _httpClient.PostAsync("signin", new StringContent(payload, Encoding.UTF8, "application/json"));
            }
            catch (Exception ex)
            {
                LogError($"[AuthService] Error POST signin: {ex}");
                throw;
            }

            var body = await response.Content.ReadAsStringAsync();

            // DEBUG
            LogInfo($"[AuthService] JSON signin response: {body}");

            if (!response.IsSuccessStatusCode)
                return null;

            SignInResponse? result = null;
            try
            {
                result = JsonSerializer.Deserialize<SignInResponse>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                LogError($"[AuthService] Error al parsear SignInResponse: {ex}");
                return null;
            }

            if (!string.IsNullOrEmpty(result?.Token))
            {
                await SaveTokenAsync(result.Token);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);

                if (!string.IsNullOrEmpty(result.UserId))
                    await SecureStorage.SetAsync(USERID_KEY, result.UserId);
            }
            return result;
        }

        public async Task<bool> IsAuthenticatedAsync()
            => !string.IsNullOrEmpty(await GetTokenAsync());

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                return await SecureStorage.GetAsync(TOKEN_KEY);
            }
            catch (Exception ex)
            {
                LogError($"[AuthService] Error GetTokenAsync: {ex}");
                return null;
            }
        }

        public async Task LogoutAsync()
        {
            try
            {
                SecureStorage.Remove(TOKEN_KEY);
                SecureStorage.Remove(USERID_KEY);
                _httpClient.DefaultRequestHeaders.Authorization = null;
            }
            catch (Exception ex)
            {
                LogError($"[AuthService] Error LogoutAsync: {ex}");
            }
        }

        public async Task SaveTokenAsync(string token)
        {
            try
            {
                await SecureStorage.SetAsync(TOKEN_KEY, token);
            }
            catch (Exception ex)
            {
                LogError($"[AuthService] Error SaveTokenAsync: {ex}");
            }
        }

        public async Task<UserInfo?> GetUserInfoAsync()
        {
            var userId = await SecureStorage.GetAsync(USERID_KEY);
            if (string.IsNullOrEmpty(userId)) return null;

            var response = await _httpClient.GetAsync($"/api/usuario/{userId}");
            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserInfo>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("register", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RefreshTokenAsync()
        {
            var refreshToken = await SecureStorage.GetAsync("refresh_token");
            if (string.IsNullOrEmpty(refreshToken))
                return false;

            var response = await _httpClient.PostAsJsonAsync("refresh-token", new { token = refreshToken });
            if (!response.IsSuccessStatusCode) return false;

            var auth = JsonSerializer.Deserialize<SignInResponse>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            if (auth?.Token is null) return false;

            await SecureStorage.SetAsync(TOKEN_KEY, auth.Token);
            await SecureStorage.SetAsync("refresh_token", auth.RefreshToken!);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);
            return true;
        }
    }

    // Modelos internos
    public class SignInRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class SignInResponse
    {
        public string Token { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Role { get; set; }  
        public string TenantId { get; set; } = string.Empty;
        public bool IsMobile { get; set; }
        public string? RefreshToken { get; set; }
    }


    public class UserInfo
    {
        public string? UserId { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? TenantId { get; set; }
        public string? Role { get; set; }
    }
}
