// AuthService.cs
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Refit;
using static ServiPuntos.Mobile.Services.AppLogger;

namespace ServiPuntos.Mobile.Services
{
    // Refit API interface
    public interface IAuthApi
    {
        [Post("/signin")]
        Task<SignInResponse> SignInAsync([Body] SignInRequest request);

        [Post("/register")]
        Task<ApiResponse<object>> RegisterAsync([Body] RegisterRequest request);

        [Get("/userinfo")]
        Task<UserInfo> GetUserInfoAsync();

        [Post("/refresh-token")]
        Task<SignInResponse> RefreshTokenAsync([Body] RefreshTokenRequest request);
    }

    // Service interface + implementation
    public interface IAuthService
    {
        Task<bool> LoginWithGoogleAsync();
        Task<SignInResponse?> SignInAsync(string email, string password);
        Task<bool> IsAuthenticatedAsync();
        Task<string?> GetTokenAsync();
        Task LogoutAsync();
        Task<UserInfo?> GetUserInfoAsync();
        Task<bool> RegisterAsync(RegisterRequest request);
        Task<bool> RefreshTokenAsync();
    }

    public class AuthService : IAuthService
    {
        private readonly IAuthApi _api;
        private readonly HttpClient _httpClient;
        private const string TOKEN_KEY = "auth_token";

        public AuthService(IAuthApi api, HttpClient httpClient)
        {
            _api = api;
            _httpClient = httpClient;
        }

        public async Task<bool> LoginWithGoogleAsync()
        {
            try
            {
                var url = new Uri(_httpClient.BaseAddress, "google-login?mobile=true").ToString();
                LogInfo($"[AuthService] Abrir navegador: {url}");
                await Browser.OpenAsync(url, BrowserLaunchMode.SystemPreferred);
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
            try
            {
                var req = new SignInRequest { Email = email, Password = password };
                var result = await _api.SignInAsync(req);

                if (!string.IsNullOrEmpty(result.Token))
                {
                    await SecureStorage.SetAsync(TOKEN_KEY, result.Token);
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", result.Token);
                }
                return result;
            }
            catch (ApiException ex)
            {
                LogError($"[AuthService] SignIn error: {ex}");
                return null;
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
            => !string.IsNullOrEmpty(await GetTokenAsync());

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                var token = await SecureStorage.GetAsync(TOKEN_KEY);
                LogInfo($"[AuthService] Token retrieved: {(token == null ? "null" : token[..10] + "...")}");
                return token;
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
                _httpClient.DefaultRequestHeaders.Authorization = null;
                LogInfo("[AuthService] Logout completed");
            }
            catch (Exception ex)
            {
                LogError($"[AuthService] Error LogoutAsync: {ex}");
            }
        }

        public async Task<UserInfo?> GetUserInfoAsync()
        {
            try
            {
                return await _api.GetUserInfoAsync();
            }
            catch (ApiException ex)
            {
                LogError($"[AuthService] GetUserInfoAsync failed: {ex}");
                return null;
            }
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            try
            {
                var response = await _api.RegisterAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (ApiException ex)
            {
                LogError($"[AuthService] RegisterAsync failed: {ex}");
                return false;
            }
        }

        public async Task<bool> RefreshTokenAsync()
        {
            try
            {
                var refreshToken = await SecureStorage.GetAsync("refresh_token");
                if (string.IsNullOrEmpty(refreshToken))
                    return false;

                var req = new RefreshTokenRequest { Token = refreshToken };
                var result = await _api.RefreshTokenAsync(req);

                if (!string.IsNullOrEmpty(result.Token))
                {
                    await SecureStorage.SetAsync(TOKEN_KEY, result.Token);
                    await SecureStorage.SetAsync("refresh_token", result.RefreshToken ?? "");
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", result.Token);
                    return true;
                }
                return false;
            }
            catch (ApiException ex)
            {
                LogError($"[AuthService] RefreshTokenAsync failed: {ex}");
                return false;
            }
        }
    }

    // DTOs
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
        public string Role { get; set; } = string.Empty;
        public string TenantId { get; set; } = string.Empty;
        public bool IsMobile { get; set; }
        public string? RefreshToken { get; set; }
    }
    public class RegisterRequest
    {
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Ci { get; set; } = string.Empty;
        public Guid TenantId { get; set; }
    }
    public class RefreshTokenRequest
    {
        public string Token { get; set; } = string.Empty;
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
