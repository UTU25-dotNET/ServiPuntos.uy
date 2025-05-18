using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Views;

public partial class HomePage : ContentPage
{
    public HomePage(TenantConfig tenant)
    {
        InitializeComponent();
        WelcomeLabel.Text = $"¡Bienvenido a {tenant.Name}!";
    }
}
