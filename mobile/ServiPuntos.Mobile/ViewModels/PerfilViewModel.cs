using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;
using System.Windows.Input;
using Microsoft.Maui.Controls;

public class PerfilViewModel : BindableObject
{
    private readonly UserService _userService;
    private readonly string _email;

    private Usuario _usuario;
    public Usuario Usuario
    {
        get => _usuario;
        set { _usuario = value; OnPropertyChanged(); }
    }

    public ICommand GuardarCommand { get; }
    public ICommand IniciarVerificacionCommand { get; }

    public PerfilViewModel(UserService userService, string email)
    {
        _userService = userService;
        _email = email;
        GuardarCommand = new Command(async () => await GuardarAsync());
        IniciarVerificacionCommand = new Command(OnIniciarVerificacion);
        CargarPerfil();
    }

    private async void CargarPerfil()
    {
        Usuario = await _userService.GetPerfilAsync(_email);
    }

    private async Task GuardarAsync()
    {
        if (Usuario != null)
        {
            var ok = await _userService.UpdatePerfilAsync(Usuario);
            await Application.Current.MainPage.DisplayAlert("Perfil", ok ? "Datos actualizados" : "No se pudo actualizar", "OK");
        }
    }

    private void OnIniciarVerificacion()
    {

        Application.Current.MainPage.DisplayAlert("Verificación", "Aquí inicia el proceso VEAI.", "OK");
    }
}
