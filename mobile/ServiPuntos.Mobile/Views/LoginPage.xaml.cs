using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.ViewModels;

namespace ServiPuntos.Mobile.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(LoginViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        void OnRegisterTapped(object? sender, System.EventArgs e)
        {
            Shell.Current.GoToAsync(nameof(RegisterPage));
        }

        void OnGoogleLoginClicked(object? sender, System.EventArgs e)
        {
            if (BindingContext is LoginViewModel vm)
                vm.GoogleLoginCommand.Execute(null);
        }
    }
}
