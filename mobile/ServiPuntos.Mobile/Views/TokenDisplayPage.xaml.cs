using System;
using System.Linq;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.Views
{
    public partial class TokenDisplayPage : ContentPage
    {
        private readonly IAuthService _authService;
        private string _currentToken = string.Empty;

        public TokenDisplayPage(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
        }

        public void SetToken(string token)
        {
            _currentToken = token;
            DisplayTokenInfo(token);
        }

        private void DisplayTokenInfo(string token)
        {
            TokenLabel.Text = token;
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var name = jsonToken.Claims.FirstOrDefault(c => c.Type == "name")?.Value ?? "No disponible";
            var email = jsonToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? "No disponible";
            var tenantId = jsonToken.Claims.FirstOrDefault(c => c.Type == "TenantId")?.Value ?? "No disponible";
            UserNameLabel.Text = $"Nombre: {name}";
            UserEmailLabel.Text = $"Email: {email}";
            TenantLabel.Text = $"Tenant ID: {tenantId}";
        }

        private async void OnCopyTokenClicked(object sender, EventArgs e)
        {
            await Clipboard.SetTextAsync(_currentToken);
            await DisplayAlert("Éxito", "Token copiado al portapapeles", "OK");
        }

        private async void OnHomeClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//PointsPage");
        }

        private async void OnViewPointsClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("//PointsPage");
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            await _authService.LogoutAsync();
            await Shell.Current.GoToAsync("//MainPage");
        }
    }
}
