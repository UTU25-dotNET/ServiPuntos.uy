using ServiPuntos.Mobile.Services;        // AuthService aquí
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace ServiPuntos.Mobile.ViewModels
{
    public class RegisterViewModel : BindableObject
    {
        private readonly IAuthService _authService;

        public RegisterViewModel(IAuthService authService)
        {
            _authService = authService;
            RegisterCommand = new Command(async () => await OnRegister());
        }

        public string Usuario { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;

        public ICommand RegisterCommand { get; }

        private async Task OnRegister()
        {
            // Referenciamos explícitamente el DTO de Services:
            var req = new ServiPuntos.Mobile.Services.RegisterRequest
            {
                Nombre = Nombre,
                Email = Email,
                Password = Password,
                // Ajusta otros campos que definas en tu RegisterRequest de Services
            };

            var ok = await _authService.RegisterAsync(req);
            if (ok)
            {
                await Application.Current.MainPage.DisplayAlert("¡Registro exitoso!", "Ahora puedes iniciar sesión.", "OK");
                await Application.Current.MainPage.Navigation.PopAsync();
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Error", "No se pudo registrar el usuario.", "OK");
            }
        }
    }
}
