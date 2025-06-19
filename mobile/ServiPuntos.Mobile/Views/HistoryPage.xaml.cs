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
            ((HistoryViewModel)BindingContext).LoadMoreCommand.Execute(null);
        }
    }
}
