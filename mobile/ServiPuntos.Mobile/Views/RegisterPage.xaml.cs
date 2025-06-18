using ServiPuntos.Mobile.ViewModels;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage(IAuthService authService)
        {
            InitializeComponent();
            BindingContext = new RegisterViewModel(authService);
        }

        private async void OnBackToLoginTapped(object sender, EventArgs e)
        {
            try
            {
                Console.WriteLine("[RegisterPage] Navegando de vuelta al login...");
                await Navigation.PopAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[RegisterPage] Error navegando al login: {ex.Message}");
                await DisplayAlert("Error", "Error al navegar al login", "OK");
            }
        }
    }
}