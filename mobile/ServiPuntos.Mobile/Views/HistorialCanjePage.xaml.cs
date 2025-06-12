using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.ViewModels;

namespace ServiPuntos.Mobile.Views
{
    public partial class HistorialCanjePage : ContentPage
    {
        public HistorialCanjePage(HistorialCanjeViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }
    }
}
