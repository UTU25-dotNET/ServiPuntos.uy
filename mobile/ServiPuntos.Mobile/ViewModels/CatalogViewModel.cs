using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.ViewModels
{
    public class CatalogViewModel : BindableObject
    {
        readonly IProductService _productService;

        public Guid LocationId { get; set; }
        public string? Categoria { get; set; }

        public ObservableCollection<CatalogItemDto> Items { get; } = new();

        bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                OnPropertyChanged();
                ((Command)LoadItemsCommand).ChangeCanExecute();
            }
        }

        public ICommand LoadItemsCommand { get; }

        public CatalogViewModel(IProductService productService)
        {
            _productService = productService;
            LoadItemsCommand = new Command(async () => await LoadItemsAsync(), () => !IsBusy);
        }

        async Task LoadItemsAsync()
        {
            if (IsBusy) return;
            try
            {
                Console.WriteLine($"[CatalogViewModel] Start LoadItems for LocationId={LocationId}, Categoria={Categoria}");
                IsBusy = true;
                Items.Clear();

                var list = await _productService.GetCatalogItemsAsync(LocationId, Categoria);
                Console.WriteLine($"[CatalogViewModel] Received {list.Count} items");
                foreach (var item in list)
                    Items.Add(item);

                if (Items.Count == 0)
                    await Application.Current.MainPage.DisplayAlert("Catálogo vacío", "No se encontraron productos para esta ubicación.", "OK");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CatalogViewModel] Exception: {ex}");
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
            finally
            {
                IsBusy = false;
                ((Command)LoadItemsCommand).ChangeCanExecute();
                Console.WriteLine($"[CatalogViewModel] End LoadItems, IsBusy={IsBusy}");
            }
        }
    }
}
