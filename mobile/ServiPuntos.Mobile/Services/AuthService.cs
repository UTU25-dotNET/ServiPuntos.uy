using System.Text.Json;
using ServiPuntos.Mobile.Views;
using static ServiPuntos.Mobile.Services.AppLogger;

namespace ServiPuntos.Mobile.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        //private const string API_BASE_URL = "http://3.15.171.220:5000/api/auth";
        private const string API_BASE_URL = "https://ec2-18-220-251-96.us-east-2.compute.amazonaws.com:5019/api/auth"; // Usar localhost para emulador Android
        private const string TOKEN_KEY = "auth_token";

        public AuthService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> LoginWithGoogleAsync()
        {
            try
            {
                var authUrl = $"{API_BASE_URL}/google-login?mobile=true";
                
                LogInfo($"[AuthService] Abriendo navegador con URL: {authUrl}");
                
                // USAR NAVEGADOR EN LUGAR DE WebAuthenticator
                await Browser.OpenAsync(authUrl, BrowserLaunchMode.SystemPreferred);
                
                LogInfo("[AuthService] Navegador abierto, esperando callback...");
                
                // Retornar true inmediatamente - el callback se manejará en App.xaml.cs
                return true;
            }
            catch (Exception ex)
            {
                LogInfo($"[AuthService] Error abriendo navegador: {ex.Message}");
                
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Error abriendo navegador: {ex.Message}", "OK");
                });
                
                return false;
            }
        }

        public async Task<bool> IsAuthenticatedAsync()
        {
            try
            {
                var token = await GetTokenAsync();
                LogInfo($"[AuthService] Verificando autenticacion: {!string.IsNullOrEmpty(token)}");
                return !string.IsNullOrEmpty(token);
            }
            catch (Exception ex)
            {
                LogInfo($"[AuthService] Error verificando autenticacion: {ex.Message}");
                return false;
            }
        }

        public async Task<string?> GetTokenAsync()
        {
            try
            {
                var token = await SecureStorage.GetAsync(TOKEN_KEY);
                LogInfo($"[AuthService] Token obtenido de SecureStorage: {(string.IsNullOrEmpty(token) ? "vacio" : "presente")}");
                return token;
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

        public async Task<UserInfo?> GetUserInfoAsync()
        {
            LogInfo("[AuthService] GetUserInfoAsync no implementado");
            return null;
        }
    }
}