using System;
using System.Net.Http;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;
using static ServiPuntos.Mobile.Services.AppLogger;

namespace ServiPuntos.Mobile.ViewModels
{
    public class LoginViewModel : BindableObject
    {
        private readonly IAuthService _authService;

        public TenantConfig Tenant { get; set; }

        private string _username = "";
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _password = "";
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private string _errorMessage = "";
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); OnPropertyChanged(nameof(HasError)); }
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public ICommand LoginCommand { get; }

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;

            Tenant = new TenantConfig
            {
                Id = "1",
                Name = "ServiPuntos",
                LogoUrl = "https://via.placeholder.com/150x100/0066CC/FFFFFF?text=ServiPuntos",
                PrimaryColor = "#0066CC",
                SecondaryColor = "#FFFFFF"
            };

            LoginCommand = new Command(async () => await ExecuteLoginAsync());
        }

        private async Task ExecuteLoginAsync()
        {
            ErrorMessage = "";
            IsLoading = true;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Completa usuario y contraseña.";
                IsLoading = false;
                return;
            }

            try
            {
                LogInfo($"[LoginVM] Iniciando SignInAsync con {Username}");
                var response = await _authService.SignInAsync(Username, Password);
                LogInfo($"[LoginVM] SignInAsync respondió: {(response == null ? "null" : "token=" + response.Token)}");

                if (response != null && !string.IsNullOrEmpty(response.Token))
                {
                    await Application.Current.MainPage.DisplayAlert("Éxito", "Login exitoso", "OK");
                    LogInfo("[LoginVM] Navegando a MainPage");
                    await Shell.Current.GoToAsync("//home"); // o la ruta que uses
                }
                else
                {
                    ErrorMessage = "Email o contraseña incorrectos.";
                    LogInfo("[LoginVM] Credenciales inválidas");
                }
            }
            catch (HttpRequestException ex)
            {
                ErrorMessage = "Email o contraseña incorrectos.";
                LogError($"[LoginVM] HttpRequestException en SignInAsync: {ex}");
            }
            catch (Exception ex)
            {
                ErrorMessage = "Error de conexión. Inténtalo de nuevo.";
                LogError($"[LoginVM] Excepción inesperada en SignInAsync: {ex}");
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
