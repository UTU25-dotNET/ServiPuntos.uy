using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Collections.ObjectModel;
using Microsoft.Maui.Controls;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;

namespace ServiPuntos.Mobile.ViewModels
{
    public class CatalogoViewModel : BindableObject
    {
        private readonly IProductoService _productoService;
        private readonly IUbicacionService _ubicacionService;


        public ObservableCollection<Ubicacion> Ubicaciones { get; }
            = new ObservableCollection<Ubicacion>();

        private Ubicacion _ubicacionSeleccionada;
        public Ubicacion UbicacionSeleccionada
        {
            get => _ubicacionSeleccionada;
            set
            {
                if (_ubicacionSeleccionada == value) return;
                _ubicacionSeleccionada = value;
                OnPropertyChanged();
                _ = CargarProductosAsync();
            }
        }

        public ObservableCollection<ProductoCanjeableDto> Productos { get; }
            = new ObservableCollection<ProductoCanjeableDto>();

        public ICommand RefreshCommand { get; }

        public CatalogoViewModel(
            IProductoService productoService,
            IUbicacionService ubicacionService)
        {
            _productoService = productoService;
            _ubicacionService = ubicacionService;

            RefreshCommand = new Command(async () => await CargarProductosAsync());
            _ = CargarUbicacionesAsync();
        }

        private async Task CargarUbicacionesAsync()
        {
            var list = await _ubicacionService.GetAllAsync();
            Ubicaciones.Clear();
            foreach (var u in list)
                Ubicaciones.Add(u);

            UbicacionSeleccionada = Ubicaciones.FirstOrDefault();
        }

        private async Task CargarProductosAsync()
        {
            if (_ubicacionSeleccionada == null) return;


            var ubicId = _ubicacionSeleccionada.Id;


            var list = await _productoService.GetProductosPorUbicacionAsync(ubicId);
            Productos.Clear();

            foreach (var dto in list)
            {

                dto.StockDisponible = await _productoService
                    .GetStockAsync(ubicId, dto.Id.ToString());

                Productos.Add(dto);
            }
        }
    }
}
