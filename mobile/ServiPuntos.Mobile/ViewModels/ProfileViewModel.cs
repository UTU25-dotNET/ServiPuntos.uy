using System.Windows.Input;
using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.ViewModels
{
    public class ProfileViewModel : BindableObject
    {
        private readonly IAuthService _authService;
        public ICommand LogoutCommand { get; }

        public ProfileViewModel(IAuthService authService)
        {
            _authService = authService;
            LogoutCommand = new Command(async () =>
            {
                await _authService.LogoutAsync();
                await Shell.Current.GoToAsync("//MainPage");
            });
        }
    }
}
