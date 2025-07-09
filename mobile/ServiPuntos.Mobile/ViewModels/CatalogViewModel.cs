using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.ViewModels
{
    public class CatalogViewModel : BindableObject
    {
        private readonly ICatalogService _catalogService;
        private bool _isBusy;
        private string? _categoria;

        public ObservableCollection<ProductoUbicacionDto> Products { get; } = new ObservableCollection<ProductoUbicacionDto>();
        public ICommand RefreshCommand { get; }

        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                OnPropertyChanged();
                ((Command)RefreshCommand).ChangeCanExecute();
            }
        }

        /// <summary>
        /// Filtro opcional de categoría.
        /// </summary>
        public string? Categoria
        {
            get => _categoria;
            set
            {
                if (_categoria == value) return;
                _categoria = value;
                OnPropertyChanged();
            }
        }

        public CatalogViewModel(ICatalogService catalogService)
        {
            _catalogService = catalogService;
            RefreshCommand = new Command(async () => await LoadAsync(), () => !IsBusy);
        }

        private async Task LoadAsync()
        {
            if (IsBusy) return;
            IsBusy = true;

            try
            {
                // Si hay categoría, podríamos filtrar localmente o
                // usar GetProductsByLocationAsync con GUID fijo. 
                // Por simplicidad traemos TODO:
                var list = await _catalogService.GetAllProductsAsync();
                Products.Clear();
                foreach (var p in list)
                    Products.Add(p);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al cargar catálogo: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
