using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.ViewModels;
using ServiPuntos.Mobile.Services;
using System.Threading.Tasks;

namespace ServiPuntos.Mobile.Views
{
    public partial class HistorialCanjePage : ContentPage
    {
        public HistorialCanjePage()
        {
            InitializeComponent();
            _ = InicializarViewModelAsync();
        }

        private async Task InicializarViewModelAsync()
        {
            var authService = Application.Current?.Handler?.MauiContext?.Services?.GetService<IAuthService>()
                              ?? new AuthService(new HttpClient());

            var userInfo = await authService.GetUserInfoAsync();
            var usuarioId = userInfo?.UserId;

            var canjeService = Application.Current?.Handler?.MauiContext?.Services?.GetService<ICanjeService>()
                               ?? new CanjeService(new HttpClient());

            BindingContext = new HistorialCanjeViewModel(canjeService, usuarioId);
        }
    }
}
