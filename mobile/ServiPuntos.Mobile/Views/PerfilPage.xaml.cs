using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.ViewModels;

namespace ServiPuntos.Mobile.Views
{
    public partial class PerfilPage : ContentPage
    {
        public PerfilPage(PerfilViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
