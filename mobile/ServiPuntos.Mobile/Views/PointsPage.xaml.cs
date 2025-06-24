using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.ViewModels;

namespace ServiPuntos.Mobile.Views
{
    public partial class PointsPage : ContentPage
    {
        PointsViewModel Vm => BindingContext as PointsViewModel;

        public PointsPage(PointsViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            Vm.RefreshCommand.Execute(null);
        }
    }
}
