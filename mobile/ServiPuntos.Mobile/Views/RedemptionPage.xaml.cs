using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.ViewModels;

namespace ServiPuntos.Mobile.Views
{
    public partial class RedemptionPage : ContentPage
    {
        public RedemptionPage(RedemptionViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            ((RedemptionViewModel)BindingContext).LoadLocationsCommand.Execute(null);
        }
    }
}
