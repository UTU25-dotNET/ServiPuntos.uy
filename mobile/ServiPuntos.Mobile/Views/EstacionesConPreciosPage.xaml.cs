using ServiPuntos.Mobile.ViewModels;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.Views
{
    public partial class EstacionesConPreciosPage : ContentPage
    {
        public EstacionesConPreciosPage()
        {
            InitializeComponent();
            InicializarViewModelAsync();
        }

        private async void InicializarViewModelAsync()
        {

            var authService = Application.Current?.Handler?.MauiContext?.Services?.GetService<IAuthService>()
                             ?? new AuthService(new HttpClient());
            var userInfo = await authService.GetUserInfoAsync();
            var tenantId = userInfo?.TenantId;

            if (string.IsNullOrEmpty(tenantId))
            {
                await DisplayAlert("Error", "No se pudo obtener el TenantId. Vuelve a iniciar sesión.", "OK");
                return;
            }

            var ubicacionService = Application.Current?.Handler?.MauiContext?.Services?.GetService<UbicacionService>()
                                   ?? new UbicacionService(new HttpClient());

            BindingContext = new EstacionesConPreciosViewModel(ubicacionService, tenantId);
        }
    }
}
