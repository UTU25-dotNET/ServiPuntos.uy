using System.Text.Json;
using System.Text;
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
            _httpClient.DefaultRequestHeaders.Add("User-Agent", "ServiPuntos.Mobile");
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

        public async Task<SignInResponse?> SignInAsync(string email, string password)
        {
            try
            {
                Console.WriteLine($"[AuthService] === INICIO SIGNIN ===");
                Console.WriteLine($"[AuthService] Email: {email}");
                Console.WriteLine($"[AuthService] Password present: {!string.IsNullOrEmpty(password)}");
                Console.WriteLine($"[AuthService] API_BASE_URL: {API_BASE_URL}");
                Console.WriteLine($"[AuthService] HttpClient configurado: {_httpClient != null}");

                var request = new SignInRequest
                {
                    Email = email,
                    Password = password
                };

                var jsonContent = JsonSerializer.Serialize(request);
                Console.WriteLine($"[AuthService] JSON serializado: {jsonContent}");
                
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                Console.WriteLine($"[AuthService] StringContent creado");
                Console.WriteLine($"[AuthService] Content-Type: {content.Headers.ContentType}");

                var url = $"{API_BASE_URL}/signin";
                Console.WriteLine($"[AuthService] URL completa: {url}");
                Console.WriteLine($"[AuthService] Iniciando petición HTTP POST...");

                var response = await _httpClient.PostAsync(url, content);
                
                Console.WriteLine($"[AuthService] Petición completada");
                Console.WriteLine($"[AuthService] Status Code: {response.StatusCode}");
                Console.WriteLine($"[AuthService] IsSuccessStatusCode: {response.IsSuccessStatusCode}");
                
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[AuthService] Response Content: {responseContent}");

                if (response.IsSuccessStatusCode)
                {
                    var signinResponse = JsonSerializer.Deserialize<SignInResponse>(responseContent, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });

                    if (signinResponse != null && !string.IsNullOrEmpty(signinResponse.Token))
                    {
                        await SaveTokenAsync(signinResponse.Token);
                        
                        _httpClient.DefaultRequestHeaders.Authorization = 
                            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", signinResponse.Token);
                    }

                    Console.WriteLine($"[AuthService] Login exitoso para: {signinResponse?.Email}");
                    return signinResponse;
                }
                else
                {
                    Console.WriteLine($"[AuthService] Error: {responseContent}");
                    throw new HttpRequestException($"Error de autenticación: {response.StatusCode}");
                }
            }
            catch (HttpRequestException httpEx)
            {
                Console.WriteLine($"[AuthService] HttpRequestException: {httpEx.Message}");
                Console.WriteLine($"[AuthService] HttpRequestException StackTrace: {httpEx.StackTrace}");
                throw;
            }
            catch (TaskCanceledException timeoutEx)
            {
                Console.WriteLine($"[AuthService] TaskCanceledException (posible timeout): {timeoutEx.Message}");
                Console.WriteLine($"[AuthService] TaskCanceledException StackTrace: {timeoutEx.StackTrace}");
                throw new HttpRequestException("Timeout al conectar con el servidor");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuthService] Exception general: {ex.GetType().Name}");
                Console.WriteLine($"[AuthService] Exception message: {ex.Message}");
                Console.WriteLine($"[AuthService] Exception StackTrace: {ex.StackTrace}");
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