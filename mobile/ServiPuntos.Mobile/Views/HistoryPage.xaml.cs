using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.ViewModels;

namespace ServiPuntos.Mobile.Views
{
    public partial class HistoryPage : ContentPage
    {
        public HistoryPage(HistoryViewModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            var vm = (HistoryViewModel)BindingContext;
            vm.ResetHistory();
            vm.LoadMoreCommand.Execute(null);
        }
    }
}
