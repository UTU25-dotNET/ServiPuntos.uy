using System.Globalization;
using System.IO;
using System.Text.Json;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Storage;
using ServiPuntos.Mobile.ViewModels;

namespace ServiPuntos.Mobile.Views
{
    public partial class MapPage : ContentPage
    {
        MapViewModel Vm => (MapViewModel)BindingContext;

        public MapPage(MapViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            // 1) Obtener ubicación actual
            var req = new GeolocationRequest(GeolocationAccuracy.Medium);
            var loc = await Geolocation.GetLocationAsync(req);
            if (loc == null)
                return;

            // 2) Cargar HTML desde el asset
            using var stream = await FileSystem.OpenAppPackageFileAsync("map.html");
            using var reader = new StreamReader(stream);
            var html = await reader.ReadToEndAsync();

            // 3) Ser algo JSON de estaciones
            var estacionesJson = JsonSerializer.Serialize(Vm.Nearby);

            // 4) Reemplazar placeholders en el HTML
            html = html
                .Replace("{{LAT}}", loc.Latitude.ToString(CultureInfo.InvariantCulture))
                .Replace("{{LNG}}", loc.Longitude.ToString(CultureInfo.InvariantCulture))
                .Replace("{{ESTACIONES_JSON}}", estacionesJson)
                .Replace("{{RADIO}}", Vm.RadioMetros.ToString(CultureInfo.InvariantCulture));

            // 5) Mostrar en WebView
            webmap.Source = new HtmlWebViewSource { Html = html };
        }
    }
}
