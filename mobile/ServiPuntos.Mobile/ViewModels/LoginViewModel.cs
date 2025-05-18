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
                await Application.Current.MainPage.Navigation.PushAsync(new Views.HomePage(Tenant));
            }
            else
            {
                ErrorMessage = "Credenciales inválidas.";
            }
        }
    }
}
