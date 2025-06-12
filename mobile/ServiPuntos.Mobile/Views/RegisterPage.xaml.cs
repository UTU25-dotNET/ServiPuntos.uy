using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.ViewModels;

namespace ServiPuntos.Mobile.Views
{
    public partial class RegisterPage : ContentPage
    {
        public RegisterPage(RegisterViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
