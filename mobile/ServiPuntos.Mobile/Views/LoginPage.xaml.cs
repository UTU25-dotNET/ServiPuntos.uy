using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.ViewModels;

namespace ServiPuntos.Mobile.Views
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage(TenantConfig tenant)
        {
            InitializeComponent();
            BindingContext = new LoginViewModel(tenant);

            // Muestra el logo y el nombre del tenant seleccionado
            LogoImage.Source = tenant.LogoUrl;
            TenantNameLabel.Text = tenant.Name;
        }
    }
}
