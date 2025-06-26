using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.ViewModels
{
    public class LocationsViewModel : BindableObject
    {
        readonly ILocationService _locationService;

        public ObservableCollection<LocationDto> Items { get; } = new();

        bool _isBusy;
        public bool IsBusy
        {
            get => _isBusy;
            set
            {
                if (_isBusy == value) return;
                _isBusy = value;
                OnPropertyChanged();
                ((Command)LoadCommand).ChangeCanExecute();
            }
        }

        public ICommand LoadCommand { get; }

        public LocationsViewModel(ILocationService locationService)
        {
            _locationService = locationService;
            LoadCommand = new Command(async () => await LoadAsync(), () => !IsBusy);
        }

        async Task LoadAsync()
        {
            if (IsBusy) return;

            try
            {
                IsBusy = true;
                Items.Clear();

                // 1) Intento tenant
                var tenantIdString = await SecureStorage.GetAsync("tenant_id");
                if (!Guid.TryParse(tenantIdString, out var tenantId))
                {
                    // Si no viene tenant_id almacenado:
                    await Application.Current.MainPage.DisplayAlert(
                        "Error",
                        "No se encontró el tenant activo",
                        "OK");
                    return;
                }

                // Traer ubicaciones para el tenant
                var list = await _locationService.GetLocationsByTenantAsync(tenantId);

                // 2) Fallback si no hay ubicaciones específicas
                if (list.Count == 0 && _locationService.GetAllLocationsAsync != null)
                {
                    list = await _locationService.GetAllLocationsAsync();
                }

                foreach (var loc in list)
                    Items.Add(loc);
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Error",
                    $"No se pudieron cargar las ubicaciones:\n{ex.Message}",
                    "OK");
            }
            finally
            {
                IsBusy = false;
                ((Command)LoadCommand).ChangeCanExecute();
            }
        }
    }
}
