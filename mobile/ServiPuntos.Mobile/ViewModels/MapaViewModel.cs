using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Devices.Sensors;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.ViewModels
{
    public class MapaViewModel : BindableObject
    {
        private readonly IUbicacionService _ubicService;

        public ObservableCollection<UbicacionDto> Estaciones { get; } = new ObservableCollection<UbicacionDto>();
        public ObservableCollection<string> Ciudades { get; } = new ObservableCollection<string>();

        private string _ciudadSeleccionada = string.Empty;
        public string CiudadSeleccionada
        {
            get => _ciudadSeleccionada;
            set
            {
                if (_ciudadSeleccionada == value) return;
                _ciudadSeleccionada = value;
                OnPropertyChanged();
            }
        }

        public double RadioKm { get; set; } = 5;

        public ICommand LoadCommand { get; }

        public MapaViewModel(IUbicacionService ubicService)
        {
            _ubicService = ubicService;
            LoadCommand = new Command(async () => await LoadAsync());
            _ = InicializarCiudadesAsync();
        }

        private async Task InicializarCiudadesAsync()
        {
            var todas = await _ubicService.GetAllAsync();
            var ciudades = todas
                .Select(u => u.Ciudad)
                .Distinct()
                .OrderBy(c => c);

            Ciudades.Clear();
            foreach (var ciudad in ciudades)
                Ciudades.Add(ciudad);

            CiudadSeleccionada = Ciudades.FirstOrDefault() ?? string.Empty;
        }

        private async Task LoadAsync()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted)
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();

            if (status != PermissionStatus.Granted)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Permiso denegado",
                    "Necesitamos tu ubicación para mostrar estaciones cercanas.",
                    "OK");
                return;
            }

            var location = await Geolocation.GetLastKnownLocationAsync()
                           ?? await Geolocation.GetLocationAsync(
                                new GeolocationRequest(
                                    GeolocationAccuracy.Medium,
                                    TimeSpan.FromSeconds(10)));

            if (location == null) return;


            var lista = await _ubicService.GetAllAsync();

            if (!string.IsNullOrEmpty(CiudadSeleccionada))
                lista = lista.Where(u => u.Ciudad == CiudadSeleccionada).ToList();

            Estaciones.Clear();
            foreach (var u in lista)
                Estaciones.Add(new UbicacionDto
                {
                    Id = Guid.Parse(u.Id),
                    Nombre = u.Nombre,
                    Ciudad = u.Ciudad,
                    Direccion = u.Direccion,

                });
        }
    }
}
