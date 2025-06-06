using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.ViewModels;
using ServiPuntos.Mobile.Services;
using static ServiPuntos.Mobile.Services.AppLogger;

namespace ServiPuntos.Mobile.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly IAuthService _authService;

        public LoginPage(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
            
            LogInfo("[LoginPage] Inicializando LoginPage...");
            SetupDefaultLogin();
        }

        private void SetupDefaultLogin()
        {
            LogInfo("[LoginPage] Configurando login por defecto...");
            TenantNameLabel.Text = "ServiPuntos";
            LogoImage.Source = "https://via.placeholder.com/150x100/0066CC/FFFFFF?text=ServiPuntos";
            
            BindingContext = new LoginViewModel();
        }

        private async void OnGoogleLoginClicked(object sender, EventArgs e)
        {
            try
            {
                LogInfo("[LoginPage] Boton Google clickeado");
                
                // DESACTIVAR EL BOTON MIENTRAS PROCESA
                var button = sender as Button;
                if (button != null)
                {
                    button.IsEnabled = false;
                    button.Text = "Autenticando...";
                }
                
                var success = await _authService.LoginWithGoogleAsync();
                
                LogInfo($"[LoginPage] Navegador abierto: {success}");
                
                if (!success)
                {
                    await DisplayAlert("Error", "Error al abrir el navegador", "OK");
                }
                // El callback se maneja en App.xaml.cs
            }
            catch (Exception ex)
            {
                LogInfo($"[LoginPage] Error: {ex.Message}");
                await DisplayAlert("Error", $"Error: {ex.Message}", "OK");
            }
            finally
            {
                // REACTIVAR EL BOTON
                var button = sender as Button;
                if (button != null)
                {
                    button.IsEnabled = true;
                    button.Text = "Iniciar sesion con Google";
                }
            }
        }
    }
}