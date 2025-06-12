using System.Linq;
using System.Collections.Specialized;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.ViewModels;

namespace ServiPuntos.Mobile.Views
{
    public partial class MapaPage : ContentPage
    {
        public MapaPage(MapaViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
            viewModel.Estaciones.CollectionChanged += OnEstacionesChanged;
        }

        void OnEstacionesChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var estaciones = ((System.Collections.ObjectModel.ObservableCollection<UbicacionDto>)sender).ToList();

            Mapa.Pins.Clear();
            foreach (var u in estaciones)
            {
                Mapa.Pins.Add(new Pin
                {
                    Label = $"{u.Nombre} (${u.PrecioNaftaSuper:F2})",
                    Address = u.Direccion,
                    Location = new Location((double)u.Latitud, (double)u.Longitud)
                });
            }

            if (estaciones.Any())
            {
                var first = estaciones.First();
                Mapa.MoveToRegion(MapSpan.FromCenterAndRadius(
                    new Location((double)first.Latitud, (double)first.Longitud),
                    Distance.FromKilometers(3)));
            }
        }
    }
}
