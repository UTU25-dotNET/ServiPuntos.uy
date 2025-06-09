using ServiPuntos.Mobile.ViewModels;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.Views
{
    public partial class PerfilPage : ContentPage
    {
        public PerfilPage()
        {
            InitializeComponent();

            var authService = Application.Current?.Handler?.MauiContext?.Services?.GetService<IAuthService>()
                              ?? new AuthService(new HttpClient());
            var userInfo = authService.GetUserInfoAsync().Result;
            var email = userInfo?.Email ?? "";

            var userService = Application.Current?.Handler?.MauiContext?.Services?.GetService<UserService>()
                             ?? new UserService(new HttpClient());
            BindingContext = new PerfilViewModel(userService, email);
        }
    }
}
