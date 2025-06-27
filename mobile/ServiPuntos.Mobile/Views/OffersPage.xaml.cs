using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.ViewModels;

namespace ServiPuntos.Mobile.Views
{
    public partial class OffersPage : ContentPage
    {
        FlashOffersViewModel Vm => BindingContext as FlashOffersViewModel;

        public OffersPage(FlashOffersViewModel vm)
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
