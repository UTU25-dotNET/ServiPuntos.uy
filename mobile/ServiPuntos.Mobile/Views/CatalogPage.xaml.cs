using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.ViewModels;

namespace ServiPuntos.Mobile.Views
{
    public partial class CatalogPage : ContentPage, IQueryAttributable
    {
        CatalogViewModel Vm => (CatalogViewModel)BindingContext;

        public CatalogPage(CatalogViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        public void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            if (query.TryGetValue("locationId", out var locObj)
                && Guid.TryParse(locObj?.ToString(), out var locId))
            {
                Console.WriteLine($"[CatalogPage] Applied locationId: {locId}");
                Vm.LocationId = locId;
            }
            else
            {
                Console.WriteLine($"[CatalogPage] locationId missing or invalid");
            }

            if (query.TryGetValue("categoria", out var catObj))
            {
                var cat = catObj?.ToString();
                Console.WriteLine($"[CatalogPage] Applied categoria: {cat}");
                Vm.Categoria = string.IsNullOrWhiteSpace(cat) ? null : cat;
            }
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (!Vm.IsBusy && Vm.Items.Count == 0)
                Vm.LoadItemsCommand.Execute(null);
        }
    }
}
