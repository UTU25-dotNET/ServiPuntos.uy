using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.Views;
using static ServiPuntos.Mobile.Services.AppLogger;
using System.Net.Http.Json;
using ServiPuntos.Mobile.Models;


namespace ServiPuntos.Mobile.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private const string TOKEN_KEY = "auth_token";

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            ConfigureHttpClient();
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "ServiPuntos.Mobile");
        }


        private void ConfigureHttpClient()
        {
#if DEBUG
            _httpClient.BaseAddress = new Uri("http://10.0.2.2:5019/api/auth");
#else
            _httpClient.BaseAddress = new Uri("https://ec2-18-220-251-96.us-east-2.compute.amazonaws.com:5019/api/auth");
#endif
        }

        public async Task<bool> LoginWithGoogleAsync()
        {
            try
            {

                var authUrl = new Uri(_httpClient.BaseAddress, "google-login?mobile=true").ToString();
                LogInfo($"[AuthService] Abriendo navegador: {authUrl}");
                await Browser.OpenAsync(authUrl, BrowserLaunchMode.SystemPreferred);
                return true;
            }
            catch (Exception ex)
            {
                LogInfo($"[AuthService] Error abriendo navegador: {ex.Message}");
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Application.Current.MainPage.DisplayAlert("Error", $"Error abriendo navegador: {ex.Message}", "OK")
                );
                return false;
            }
        }

        public async Task<SignInResponse?> SignInAsync(string email, string password)
        {
            var request = new SignInRequest { Email = email, Password = password };
            var content = new StringContent(
                JsonSerializer.Serialize(request),
                Encoding.UTF8,
                "application/json"
            );


            var response = await _httpClient.PostAsync("signin", content);
            var responseContent = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                throw new HttpRequestException($"Error de autenticación: {response.StatusCode}");

            var signinResponse = JsonSerializer.Deserialize<SignInResponse>(
                responseContent,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (signinResponse is not null && !string.IsNullOrEmpty(signinResponse.Token))
            {
                await SaveTokenAsync(signinResponse.Token);
                _httpClient.DefaultRequestHeaders.Authorization =
                    new AuthenticationHeaderValue("Bearer", signinResponse.Token);
            }

            return signinResponse;
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
                LogInfo($"[AuthService] Error obteniendo token: {ex.Message}");
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
                LogInfo($"[AuthService] Error en logout: {ex.Message}");
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
                LogInfo($"[AuthService] Error guardando token: {ex.Message}");
            }
        }

        public async Task<UserInfo?> GetUserInfoAsync()
        {

            var response = await _httpClient.GetAsync("profile");
            if (!response.IsSuccessStatusCode) return null;
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<UserInfo>(
                json,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
        }

        public async Task<bool> RegisterAsync(RegisterRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", request);
            return response.IsSuccessStatusCode;
        }
    }
}
