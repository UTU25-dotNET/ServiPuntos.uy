using System.Windows.Input;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.ViewModels
{
    public class LoginViewModel : BindableObject
    {
        private readonly IAuthService _authService;
        // private string _username = string.Empty;
        // private string _password = string.Empty;
        // private string _errorMessage = string.Empty;


        public TenantConfig Tenant { get; set; }

        private string _username;
        public string Username
        {
            get => _username;
            set { _username = value; OnPropertyChanged(); }
        }

        private string _password;
        public string Password
        {
            get => _password;
            set { _password = value; OnPropertyChanged(); }
        }

        private string _errorMessage;
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

        // Constructor que recibe AuthService por inyección de dependencias
        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
            
            // Crear un tenant por defecto
            Tenant = new TenantConfig
            {
                Id = "1",
                Name = "ServiPuntos",
                LogoUrl = "https://via.placeholder.com/150x100/0066CC/FFFFFF?text=ServiPuntos",
                PrimaryColor = "#0066CC",
                SecondaryColor = "#FFFFFF"
            };
            
            LoginCommand = new Command(OnLogin);
        }

        private async void OnLogin()
        {
            Console.WriteLine($"[LoginViewModel] === INICIO OnLogin ===");
            Console.WriteLine($"[LoginViewModel] Username: {Username}");
            Console.WriteLine($"[LoginViewModel] Password present: {!string.IsNullOrEmpty(Password)}");
            Console.WriteLine($"[LoginViewModel] AuthService: {_authService != null}");

            ErrorMessage = "";
            IsLoading = true;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Completa usuario y contraseña.";
                IsLoading = false;
                Console.WriteLine("[LoginViewModel] Error: Campos vacíos");
                return;
            }

            try
            {
                Console.WriteLine("[LoginViewModel] Llamando a SignInAsync...");
                
                var response = await _authService.SignInAsync(Username, Password);
                
                Console.WriteLine($"[LoginViewModel] Respuesta recibida: {response != null}");

                if (response != null)
                {
                    Console.WriteLine($"[LoginViewModel] Token recibido: {!string.IsNullOrEmpty(response.Token)}");
                    
                    // Login exitoso
                    await Application.Current.MainPage.DisplayAlert("Éxito", "Login exitoso", "OK");
                    
                    // Navegar a la página principal
                    // await Shell.Current.GoToAsync("//main");
                }
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"[LoginViewModel] HttpRequestException: {ex.Message}");
                ErrorMessage = "Email o contraseña incorrectos.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LoginViewModel] Exception: {ex.Message}");
                Console.WriteLine($"[LoginViewModel] Exception StackTrace: {ex.StackTrace}");
                ErrorMessage = "Error de conexión. Inténtalo de nuevo.";
            }
            finally
            {
                IsLoading = false;
                Console.WriteLine("[LoginViewModel] === FIN OnLogin ===");
            }
        }
    }
}