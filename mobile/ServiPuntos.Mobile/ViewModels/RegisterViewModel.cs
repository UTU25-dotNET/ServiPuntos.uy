using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;
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

        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Ci { get; set; } = string.Empty;


        public string TenantId { get; set; } = "b3908dc4-79b4-4e6f-9e15-6db86e10baaa";

        public ICommand RegisterCommand { get; }

        private async Task OnRegister()
        {
            if (string.IsNullOrWhiteSpace(Nombre) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(Password) ||
                string.IsNullOrWhiteSpace(Ci) ||
                string.IsNullOrWhiteSpace(TenantId))
            {
                await Application.Current.MainPage.DisplayAlert("Error", "Completa todos los campos.", "OK");
                return;
            }

            var req = new RegisterRequest
            {
                Nombre = this.Nombre,
                Email = this.Email,
                Password = this.Password,
                Ci = this.Ci,
                TenantId = this.TenantId 
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
