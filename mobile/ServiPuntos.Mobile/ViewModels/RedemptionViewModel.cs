using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;
using ServiPuntos.Mobile.Views;

namespace ServiPuntos.Mobile.ViewModels
{
    public class RedemptionViewModel : BindableObject
    {
        readonly ICanjeService _canjeService;
        readonly IProductService _productService;
        readonly ILocationService _locationService;

        public ObservableCollection<LocationDto> Locations { get; } = new();
        public ObservableCollection<ProductDto> Products { get; } = new();

        LocationDto _selectedLocation;
        public LocationDto SelectedLocation
        {
            get => _selectedLocation;
            set
            {
                if (_selectedLocation == value) return;
                _selectedLocation = value;
                OnPropertyChanged();
                ((Command)LoadProductsCommand).ChangeCanExecute();
                LoadProductsCommand.Execute(null);
            }
        }

        ProductDto _selectedProduct;
        public ProductDto SelectedProduct
        {
            get => _selectedProduct;
            set
            {
                if (_selectedProduct == value) return;
                _selectedProduct = value;
                OnPropertyChanged();
                ((Command)GenerateCommand).ChangeCanExecute();
            }
        }

        bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                OnPropertyChanged();
                ((Command)LoadLocationsCommand).ChangeCanExecute();
                ((Command)LoadProductsCommand).ChangeCanExecute();
                ((Command)GenerateCommand).ChangeCanExecute();
            }
        }

        public ICommand LoadLocationsCommand { get; }
        public ICommand LoadProductsCommand { get; }
        public ICommand GenerateCommand { get; }

        public RedemptionViewModel(
            ICanjeService canjeService,
            IProductService productService,
            ILocationService locationService)
        {
            _canjeService = canjeService;
            _productService = productService;
            _locationService = locationService;

            LoadLocationsCommand = new Command(
                async () => await LoadLocationsAsync(),
                () => !IsBusy);

            LoadProductsCommand = new Command(
                async () => await LoadProductsAsync(),
                () => !IsBusy && SelectedLocation != null);

            GenerateCommand = new Command(
                async () => await GenerateAsync(),
                () => !IsBusy && SelectedProduct != null);
        }

        async Task LoadLocationsAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                Locations.Clear();
                var tenantId = Guid.Parse(await SecureStorage.GetAsync("tenant_id"));
                var locs = await _locationService.GetLocationsByTenantAsync(tenantId);
                foreach (var l in locs)
                    Locations.Add(l);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task LoadProductsAsync()
        {
            if (IsBusy || SelectedLocation == null)
                return;

            try
            {
                IsBusy = true;
                Products.Clear();
                var prods = await _productService.GetProductsByLocationAsync(SelectedLocation.Id);
                foreach (var p in prods)
                    Products.Add(p);
            }
            finally
            {
                IsBusy = false;
            }
        }

        async Task GenerateAsync()
        {
            if (IsBusy || SelectedProduct == null)
                return;

            try
            {
                IsBusy = true;
                var userId = Guid.Parse(await SecureStorage.GetAsync("userId"));
                var tenantId = Guid.Parse(await SecureStorage.GetAsync("tenant_id"));
                var qrCode = await _canjeService.GenerarCanjeAsync(
                    userId,
                    SelectedProduct.ProductoCanjeableId,
                    SelectedLocation.Id,
                    tenantId);

                var encoded = Uri.EscapeDataString(qrCode);
                await Shell.Current.GoToAsync($"//{nameof(QRCodePage)}?code={encoded}");

            }
            catch (HttpRequestException httpEx)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error de red",
                    $"No se pudo generar el canje:\n{httpEx.Message}",
                    "OK");
            }
            catch (InvalidOperationException opEx)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error al generar el canje",
                    opEx.Message,
                    "OK");
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error inesperado",
                    ex.Message,
                    "OK");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}
