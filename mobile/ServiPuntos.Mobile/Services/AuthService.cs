using System.Text.Json;
using System.Text;
using ServiPuntos.Mobile.Views;
using static ServiPuntos.Mobile.Services.AppLogger;

namespace ServiPuntos.Mobile.Services
{
    public class AuthService : IAuthService
    {
        private readonly HttpClient _httpClient;
        private const string API_BASE_URL = "https://ec2-18-220-251-96.us-east-2.compute.amazonaws.com:5019/api/auth"; // HTTPS

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
                {
                    await Application.Current.MainPage.DisplayAlert("Error", $"Error abriendo navegador: {ex.Message}", "OK");
                });
                
                return false;
            }
        }
        

        public async Task<SignInResponse?> SignInAsync(string email, string password)
        {
            try
            {
                Console.WriteLine($"[AuthService] === INICIO SIGNIN ===");
                Console.WriteLine($"[AuthService] Email: {email}");
                Console.WriteLine($"[AuthService] Password present: {!string.IsNullOrEmpty(password)}");
                Console.WriteLine($"[AuthService] API_BASE_URL: {API_BASE_URL}");

                // Crear cliente que acepte auto-firmados en desarrollo
                var handler = new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback =
                        HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
                using var client = new HttpClient(handler);

                // Serializar payload
                var data = new { email, password };
                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Llamada POST
                HttpResponseMessage response;
                try
                {
                    response = await client.PostAsync($"{API_BASE_URL}/signin", content);
                    var debugBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"STATUS: {response.StatusCode} - BODY: {debugBody}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AuthService] ERROR llamando al API: {ex.Message}");
                    throw;
                }

                // Leer respuesta
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[AuthService] Response Content: {responseContent}");

                if (!response.IsSuccessStatusCode)
                    throw new HttpRequestException($"Error de autenticación: {response.StatusCode}");

                // Parseo manual para ajustar tipos
                using var doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;
                var signinResponse = new SignInResponse
                {
                    Token    = root.GetProperty("token").GetString()!,
                    UserId   = root.GetProperty("userId").GetString()!,
                    Username = root.GetProperty("username").GetString()!,
                    Email    = root.GetProperty("email").GetString()!,
                    Role     = root.GetProperty("role").GetInt32().ToString(),        // ahora int
                    TenantId = root.GetProperty("tenantId").GetString()!
                };

                // Guardar token y asignar header
                await SaveTokenAsync(signinResponse.Token);
                _httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", signinResponse.Token);

                Console.WriteLine($"[AuthService] Login exitoso para: {signinResponse.Email}");

                // —— NAVEGACIÓN A TokenDisplayPage ——  
                await MainThread.InvokeOnMainThreadAsync(async () =>
                {
                    var tokenDisplayPage = new TokenDisplayPage(this);
                    tokenDisplayPage.SetToken(signinResponse.Token);

                    Console.WriteLine("[AuthService] Navegando a TokenDisplayPage...");
                    await Application.Current.MainPage.Navigation.PushAsync(tokenDisplayPage); //Esto despues hay que tocarlo y redirigir a donde queramos
                    Console.WriteLine("[AuthService] Navegación completada");
                });
                return signinResponse;
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"[AuthService] HttpRequestException: {httpEx.Message}");
                throw;
            }
            catch (TaskCanceledException timeoutEx)
            {
                Console.WriteLine($"[AuthService] Timeout: {timeoutEx.Message}");
                throw new HttpRequestException("Timeout al conectar con el servidor");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthService] Exception general: {ex.GetType().Name} - {ex.Message}");
                throw;
            }
        }

        public void Dispose()
        {
            _httpClient?.Dispose();
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
            LogInfo("[AuthService] GetUserInfoAsync no implementado");
            return null;
        }
    }
}