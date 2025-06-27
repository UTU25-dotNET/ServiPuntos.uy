using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.ViewModels;

namespace ServiPuntos.Mobile.Views
{
    public partial class AlertsPage : ContentPage
    {
        AlertsViewModel Vm => BindingContext as AlertsViewModel;

        public AlertsPage(AlertsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Vm.RefreshCommand.Execute(null);
        }
    }
}
