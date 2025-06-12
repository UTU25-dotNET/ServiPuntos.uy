using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.ViewModels;

namespace ServiPuntos.Mobile.Views
{
    public partial class CatalogoPage : ContentPage
    {
        public CatalogoPage(CatalogoViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
