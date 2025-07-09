using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;
using Microsoft.Maui.Storage;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;
using ServiPuntos.Mobile.Views;
using static ServiPuntos.Mobile.Services.AppLogger;

namespace ServiPuntos.Mobile.ViewModels
{
    public class LoginViewModel : BindableObject
    {
        private readonly IAuthService _authService;
        private readonly ITenantService _tenantService;
        private readonly IUserService _userService;
        private readonly PushNotificationService _pushService;

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
        public ICommand GoogleLoginCommand { get; }

        public LoginViewModel(
            IAuthService authService,
            ITenantService tenantService,
            IUserService userService,
            PushNotificationService pushService)
        {
            _authService = authService;
            _tenantService = tenantService;
            _userService = userService;
            _pushService = pushService;

            Tenant = new TenantConfig
            {
                Id = "1",
                Name = "ServiPuntos",
                LogoUrl = "https://via.placeholder.com/150x100/0066CC/FFFFFF?text=ServiPuntos",
                PrimaryColor = "#0066CC",
                SecondaryColor = "#FFFFFF"
            };

            LoginCommand = new Command(OnLogin);
            GoogleLoginCommand = new Command(async () => await _authService.LoginWithGoogleAsync());
        }

        private async void OnLogin()
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
                var response = await _authService.SignInAsync(Username, Password);
                if (response != null)
                {
                    // Aplicar colores del tenant
                    var tenant = await _tenantService.GetByIdAsync(Guid.Parse(response.TenantId));
                    Application.Current.Resources["PrimaryColor"] =
                        Color.FromArgb(tenant.PrimaryColor);
                    LogInfo($"[LoginViewModel] PrimaryColor actualizado a {tenant.PrimaryColor}");

                    // Registrar token FCM y enviarlo al backend
                    var fcmToken = await _pushService.RegisterAndRetrieveTokenAsync();
                    LogInfo($"[LoginViewModel] FCM token adquirido: {!string.IsNullOrEmpty(fcmToken)}");
                    await SendFcmTokenAsync();

                    await Application.Current.MainPage.DisplayAlert("Éxito", "Login exitoso", "OK");
                    // Navegamos al Tab “Saldo”
                    await Shell.Current.GoToAsync($"//{nameof(PointsPage)}");
                }
            }
            catch (HttpRequestException)
            {
                ErrorMessage = "Email o contraseña incorrectos.";
            }
            catch (Exception)
            {
                ErrorMessage = "Error de conexión. Inténtalo de nuevo.";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private async Task SendFcmTokenAsync()
        {
            try
            {
                var token = await _pushService.GetStoredTokenAsync();
                LogInfo($"[LoginViewModel] FCM token presente: {!string.IsNullOrEmpty(token)}");
                if (!string.IsNullOrEmpty(token))
                {
                    await _userService.UpdateFcmTokenAsync(token);
                    LogInfo("[LoginViewModel] FCM token enviado al API");
                }
            }
            catch (Exception ex)
            {
                LogInfo($"[LoginViewModel] Error enviando FCM token: {ex.Message}");
            }
        }
    }
}