using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.ViewModels;

namespace ServiPuntos.Mobile.Views
{
    public partial class CatalogPage : ContentPage
    {
        private CatalogViewModel Vm => (CatalogViewModel)BindingContext;

        public CatalogPage(CatalogViewModel vm)
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
