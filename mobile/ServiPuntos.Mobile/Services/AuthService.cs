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
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private const string TOKEN_KEY = "auth_token";

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "ServiPuntos.Mobile");
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
            LogInfo($"[AuthService] POST {_httpClient.BaseAddress}signin");
            var req = new SignInRequest { Email = email, Password = password };
            var payload = JsonSerializer.Serialize(req);
            LogInfo($"[AuthService] Payload: {payload}");

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

            LogInfo($"[AuthService] StatusCode: {response.StatusCode}");
            var body = await response.Content.ReadAsStringAsync();
            LogInfo($"[AuthService] ResponseBody: {body}");

            if (!response.IsSuccessStatusCode)
            {
                LogInfo("[AuthService] Signin falló, devolviendo null");
                return null;
            }

            SignInResponse? result;
            try
            {
                result = JsonSerializer.Deserialize<SignInResponse>(body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch (Exception ex)
            {
                LogError($"[AuthService] Error al parsear SignInResponse: {ex}");
                return null;
            }

            if (result?.Token is string token && token.Length > 0)
            {
                LogInfo("[AuthService] Guardando token");
                await SaveTokenAsync(token);
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            else
            {
                LogInfo("[AuthService] SignInResponse sin token");
            }

            return result;
        }

        public async Task<bool> IsAuthenticatedAsync()
            => !string.IsNullOrEmpty(await GetTokenAsync());

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                var t = await SecureStorage.GetAsync(TOKEN_KEY);
                LogInfo($"[AuthService] GetTokenAsync -> {(t == null ? "null" : t[..10] + "...")}");
                return t;
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
                LogInfo("[AuthService] Logout completado");
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
                LogInfo("[AuthService] Token guardado exitosamente");
            }
            catch (Exception ex)
            {
                LogError($"[AuthService] Error SaveTokenAsync: {ex}");
            }
        }

        private const string USER_INFO_PATH = "../usuario/me";
        public async Task<UserInfo?> GetUserInfoAsync()
        {
            LogInfo("[AuthService] GET USERINFO");
            var response = await _httpClient.GetAsync(USER_INFO_PATH);
            if (!response.IsSuccessStatusCode)
            {
                LogInfo($"[AuthService] GetUserInfoAsync falló: {response.StatusCode}");
                return null;
            }
            var json = await response.Content.ReadAsStringAsync();
            LogInfo($"[AuthService] UserInfo body: {json}");
            return JsonSerializer.Deserialize<UserInfo>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            LogInfo("[AuthService] POST register");
            var response = await _httpClient.PostAsJsonAsync("register", request);
            LogInfo($"[AuthService] Register Status: {response.StatusCode}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> RefreshTokenAsync()
        {
            LogInfo("[AuthService] POST refresh-token");
            var refreshToken = await SecureStorage.GetAsync("refresh_token");
            if (string.IsNullOrEmpty(refreshToken))
            {
                LogInfo("[AuthService] No hay refresh_token");
                return false;
            }

            var response = await _httpClient.PostAsJsonAsync("refresh-token", new { token = refreshToken });
            LogInfo($"[AuthService] RefreshToken Status: {response.StatusCode}");
            if (!response.IsSuccessStatusCode) return false;

            var auth = JsonSerializer.Deserialize<SignInResponse>(
                await response.Content.ReadAsStringAsync(),
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            if (auth?.Token is null) return false;

            await SecureStorage.SetAsync("auth_token", auth.Token);
            await SecureStorage.SetAsync("refresh_token", auth.RefreshToken!);
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", auth.Token);
            LogInfo("[AuthService] RefreshToken completado");
            return true;
        }
    }
}
