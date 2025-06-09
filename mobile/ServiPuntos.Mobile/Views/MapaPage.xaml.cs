using ServiPuntos.Mobile.ViewModels;
using ServiPuntos.Mobile.Services;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Linq;
using System.Collections.Specialized;

namespace ServiPuntos.Mobile.Views
{
    public partial class MapaPage : ContentPage
    {
        public MapaPage()
        {
            InitializeComponent();
            InicializarViewModelAsync();
        }

        private async void InicializarViewModelAsync()
        {

            var authService = Application.Current?.Handler?.MauiContext?.Services?.GetService<IAuthService>()
                             ?? new AuthService(new HttpClient());
            var userInfo = await authService.GetUserInfoAsync();
            var tenantId = userInfo?.TenantId;

            if (string.IsNullOrEmpty(tenantId))
            {
                await DisplayAlert("Error", "No se pudo obtener el TenantId. Reintenta iniciar sesión.", "OK");
                return;
            }

            var ubicacionService = Application.Current?.Handler?.MauiContext?.Services?.GetService<UbicacionService>()
                                   ?? new UbicacionService(new HttpClient());
            var viewModel = new MapaViewModel(ubicacionService, tenantId);
            BindingContext = viewModel;

            viewModel.Ubicaciones.CollectionChanged += Ubicaciones_CollectionChanged;
        }

        private void Ubicaciones_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var ubicaciones = ((System.Collections.ObjectModel.ObservableCollection<ServiPuntos.Mobile.Models.Ubicacion>)sender).ToList();
            Mapa.Pins.Clear();
            foreach (var u in ubicaciones)
            {
                var pin = new Pin
                {
                    Label = $"{u.Nombre} (${u.PrecioNaftaSuper:F2})",
                    Address = u.Direccion,
                    Location = new Location(u.Latitud, u.Longitud)
                };
                Mapa.Pins.Add(pin);
            }
            if (ubicaciones.Any())
            {
                var first = ubicaciones.First();
                Mapa.MoveToRegion(MapSpan.FromCenterAndRadius(
                    new Location(first.Latitud, first.Longitud),
                    Distance.FromKilometers(3)));
            }
        }
    }
}
