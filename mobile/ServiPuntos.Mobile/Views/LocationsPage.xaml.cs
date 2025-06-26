using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.ViewModels;
using System;

namespace ServiPuntos.Mobile.Views
{
    public partial class LocationsPage : ContentPage
    {
        LocationsViewModel Vm => (LocationsViewModel)BindingContext;

        public LocationsPage(LocationsViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!Vm.IsBusy && Vm.Items.Count == 0)
                Vm.LoadCommand.Execute(null);
        }

        async void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.CurrentSelection.Count == 0)
                return;

            var selected = (LocationDto)e.CurrentSelection[0];
            Console.WriteLine($"[LocationsPage] Selected LocationId: {selected.Id}");
            await Shell.Current.GoToAsync($"//CatalogPage?locationId={selected.Id}");
            ((CollectionView)sender).SelectedItem = null;
        }
    }
}
