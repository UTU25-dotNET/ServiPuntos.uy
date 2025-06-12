using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.ViewModels;

namespace ServiPuntos.Mobile.Views
{
    public partial class HomePage : ContentPage
    {
        public HomePage(HomeViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
