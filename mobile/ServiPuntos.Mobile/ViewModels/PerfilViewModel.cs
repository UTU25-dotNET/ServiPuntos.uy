using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.ViewModels
{
    public class PerfilViewModel : BindableObject
    {
        readonly IAuthService _authService;
        readonly IUserService _userService;
        readonly IVeaiService _veaiService;

        public Usuario Usuario { get; private set; } = new Usuario();

        private string _estadoVEAI = "";
        public string EstadoVEAI
        {
            get => _estadoVEAI;
            set { _estadoVEAI = value; OnPropertyChanged(); }
        }

        public ICommand GuardarCommand { get; }
        public ICommand IniciarVerificacionCommand { get; }

        public PerfilViewModel(
            IAuthService authService,
            IUserService userService,
            IVeaiService veaiService)
        {
            _authService = authService;
            _userService = userService;
            _veaiService = veaiService;

            GuardarCommand = new Command(async () => await GuardarAsync());
            IniciarVerificacionCommand = new Command(async () => await OnIniciarVerificacionAsync());

            _ = CargarPerfilAsync();
        }

        private async Task CargarPerfilAsync()
        {
            var info = await _authService.GetUserInfoAsync();
            if (info?.Email == null) return;

            Usuario = await _userService.GetPerfilByEmailAsync(info.Email)
                      ?? new Usuario();
            OnPropertyChanged(nameof(Usuario));
        }

        private async Task OnIniciarVerificacionAsync()
        {

            var cedula = Usuario.Ci.ToString();
            if (string.IsNullOrWhiteSpace(cedula))
            {
                await Application.Current.MainPage
                    .DisplayAlert("Verificación", "Por favor ingresa tu cédula.", "OK");
                return;
            }

            var result = await _veaiService.VerifyAgeAsync(cedula);
            if (result == null)
            {
                await Application.Current.MainPage
                    .DisplayAlert("Verificación", "Error al verificar.", "OK");
                return;
            }

            Usuario.VerificadoVEAI = result.IsAllowed;

            EstadoVEAI = result.IsAllowed
                ? $"Verificado (edad {result.Edad})"
                : $"No permitido (edad {result.Edad})";


            await _userService.UpdatePerfilAsync(Usuario);
        }

        private async Task GuardarAsync()
        {
            var ok = await _userService.UpdatePerfilAsync(Usuario);
            await Application.Current.MainPage
                .DisplayAlert("Perfil", ok ? "Datos actualizados" : "No se pudo actualizar", "OK");
        }
    }
}
