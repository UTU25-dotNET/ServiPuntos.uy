using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.ViewModels;

namespace ServiPuntos.Mobile.Views
{
    public partial class EstacionesConPreciosPage : ContentPage
    {
        public EstacionesConPreciosPage(EstacionesConPreciosViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
