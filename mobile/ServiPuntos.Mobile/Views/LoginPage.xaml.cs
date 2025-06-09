using System;
using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.Services;
using ServiPuntos.Mobile.ViewModels;
using static ServiPuntos.Mobile.Services.AppLogger;

namespace ServiPuntos.Mobile.Views
{
    public partial class LoginPage : ContentPage
    {
        private readonly IAuthService _authService;

        public LoginPage(LoginViewModel viewModel, IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
            BindingContext = viewModel;
        }

        private async void OnGoogleLoginClicked(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                try
                {
                    LogInfo("[LoginPage] Google login iniciado");
                    button.IsEnabled = false;
                    button.Text = "Autenticando...";

                    var success = await _authService.LoginWithGoogleAsync();
                    if (!success)
                        await DisplayAlert("Error", "No se pudo abrir el navegador", "OK");
                }
                catch (Exception ex)
                {
                    LogInfo($"[LoginPage] Error: {ex.Message}");
                    await DisplayAlert("Error", ex.Message, "OK");
                }
                finally
                {
                    button.IsEnabled = true;
                    button.Text = "Iniciar sesión con Google";
                }
            }
        }
    }
}
