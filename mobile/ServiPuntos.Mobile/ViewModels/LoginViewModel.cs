using System.Windows.Input;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.ViewModels
{
    public class LoginViewModel : BindableObject
    {
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

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        public ICommand LoginCommand { get; }

        // **CORREGIDO: Constructor por defecto sin parámetros**
        public LoginViewModel()
        {
            // Crear un tenant por defecto
            Tenant = new TenantConfig
            {
                Id = "1", // **CORREGIDO: String en lugar de int**
                Name = "ServiPuntos",
                LogoUrl = "https://via.placeholder.com/150x100/0066CC/FFFFFF?text=ServiPuntos",
                PrimaryColor = "#0066CC",
                SecondaryColor = "#FFFFFF"
            };
            
            LoginCommand = new Command(OnLogin);
        }

        // **MANTENER: Constructor con tenant (por si se necesita más adelante)**
        public LoginViewModel(TenantConfig tenant)
        {
            Tenant = tenant;
            LoginCommand = new Command(OnLogin);
        }

        private async void OnLogin()
        {
            ErrorMessage = "";

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "Completa usuario y contraseña.";
                return;
            }

            // Mock login válido
            if (Username == "admin" && Password == "1234")
            {
                // Por ahora, mostrar un mensaje de éxito
                await Application.Current.MainPage.DisplayAlert("Éxito", "Login tradicional exitoso", "OK");
            }
            else
            {
                ErrorMessage = "Credenciales inválidas.";
            }
        }
    }
}