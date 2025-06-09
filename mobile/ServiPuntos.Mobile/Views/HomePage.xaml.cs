using ServiPuntos.Mobile.ViewModels;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage()
        {
            InitializeComponent();
            var userService = Application.Current?.Handler?.MauiContext?.Services?.GetService<UserService>()
                              ?? new UserService(new HttpClient());
            BindingContext = new HomeViewModel(userService);
        }
    }
}
