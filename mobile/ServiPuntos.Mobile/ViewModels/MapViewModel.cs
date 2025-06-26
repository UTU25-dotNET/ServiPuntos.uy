using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.ViewModels
{
    public class MapViewModel : BindableObject
    {
        readonly ILocationService _locationService;
        double _latitude;
        double _longitude;

        public ObservableCollection<LocationDto> Nearby { get; } = new();

        public List<string> Servicios { get; } = new() { "LavadoDeAuto", "CambioDeAceite", "CambioDeNeumaticos" };
        public List<string> Combustibles { get; } = new() { "NaftaSuper", "NaftaPremium", "Diesel" };

        string? _servicioFiltro;
        public string? ServicioFiltro
        {
            get => _servicioFiltro;
            set { _servicioFiltro = value; OnPropertyChanged(); }
        }

        string? _combustibleFiltro;
        public string? CombustibleFiltro
        {
            get => _combustibleFiltro;
            set { _combustibleFiltro = value; OnPropertyChanged(); }
        }

        int _radioMetros = 1000;
        public int RadioMetros
        {
            get => _radioMetros;
            set { _radioMetros = value; OnPropertyChanged(); }
        }

        public ICommand LoadNearbyCommand { get; }

        public MapViewModel(ILocationService locationService)
        {
            _locationService = locationService;
            LoadNearbyCommand = new Command(async () => await LoadNearbyAsync());
        }

        public void SetLocation(double latitude, double longitude)
        {
            _latitude = latitude;
            _longitude = longitude;
        }

        async Task LoadNearbyAsync()
        {
            var list = await _locationService.GetNearbyAsync(
                _latitude, _longitude, ServicioFiltro, CombustibleFiltro, RadioMetros);
            Nearby.Clear();
            foreach (var loc in list)
                Nearby.Add(loc);
        }
    }
}
