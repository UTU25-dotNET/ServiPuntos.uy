using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

namespace ServiPuntos.Mobile.ViewModels
{
    public class RegisterViewModel : BindableObject
    {
        private readonly AuthService _authService;

        public RegisterViewModel(AuthService authService)
        {
            _authService = authService;
            RegisterCommand = new Command(async () => await OnRegister());
        }

        public string Usuario { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Nombre { get; set; }

        public ICommand RegisterCommand { get; }

        private async Task OnRegister()
        {
            var req = new RegisterRequest
            {
                Usuario = this.Usuario,
                Password = this.Password,
                Email = this.Email,
                Nombre = this.Nombre
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
