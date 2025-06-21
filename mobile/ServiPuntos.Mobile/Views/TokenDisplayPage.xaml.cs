using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.Views
{
    public partial class TokenDisplayPage : ContentPage
    {
        private readonly IAuthService _authService;
        private string _currentToken = string.Empty;

        public TokenDisplayPage(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        // Método para configurar el token recibido
        public void SetToken(string token)
        {
            _currentToken = token;
            DisplayTokenInfo(token);
        }

        private void DisplayTokenInfo(string token)
        {
            try
            {
                // Mostrar el token completo
                TokenLabel.Text = token;

                // Decodificar el JWT para mostrar información del usuario
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(token);

                // Extraer información del usuario
                var name = jsonToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "No disponible";
                var email = jsonToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "No disponible";
                var tenantId = jsonToken.Claims.FirstOrDefault(c => c.Type == "TenantId")?.Value ?? "No disponible";

                // Actualizar labels
                UserNameLabel.Text = $"Nombre: {name}";
                UserEmailLabel.Text = $"Email: {email}";
                TenantLabel.Text = $"Tenant ID: {tenantId}";

                Console.WriteLine($"[TokenDisplay] Token decodificado exitosamente");
                Console.WriteLine($"[TokenDisplay] Usuario: {name} ({email})");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TokenDisplay] Error al decodificar token: {ex.Message}");
                
                // Mostrar información básica sin decodificar
                UserNameLabel.Text = "Error al decodificar información del usuario";
                UserEmailLabel.Text = "";
                TenantLabel.Text = "";
            }
        }

        private async void OnCopyTokenClicked(object sender, EventArgs e)
        {
            try
            {
                await Clipboard.SetTextAsync(_currentToken);
                await DisplayAlert("Éxito", "Token copiado al portapapeles", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"No se pudo copiar el token: {ex.Message}", "OK");
            }
        }

        private async void OnHomeClicked(object sender, EventArgs e)
        {
            try
            {
                // Navegar al dashboard o página principal
                await Shell.Current.GoToAsync("//main");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TokenDisplay] Error navegando al home: {ex.Message}");
                await DisplayAlert("Error", "No se pudo navegar al dashboard", "OK");
            }
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            try
            {
                await _authService.LogoutAsync();
                await Shell.Current.GoToAsync("//login");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[TokenDisplay] Error en logout: {ex.Message}");
                await DisplayAlert("Error", "No se pudo cerrar sesión", "OK");
            }
        }
    }
}