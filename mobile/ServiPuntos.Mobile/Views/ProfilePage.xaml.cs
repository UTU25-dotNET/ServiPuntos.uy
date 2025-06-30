using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.ViewModels;

namespace ServiPuntos.Mobile.Views
{
    public partial class ProfilePage : ContentPage
    {
        public ProfilePage(ProfileViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}
