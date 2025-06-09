using System.Collections.ObjectModel;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;

public class CatalogoViewModel : BindableObject
{
    private readonly ProductoService _productoService;
    private readonly UbicacionService _ubicacionService;
    private readonly string _tenantId;

    public ObservableCollection<ProductoCanjeable> Productos { get; set; } = new();
    public ObservableCollection<Ubicacion> Ubicaciones { get; set; } = new();

    private Ubicacion _ubicacionSeleccionada;
    public Ubicacion UbicacionSeleccionada
    {
        get => _ubicacionSeleccionada;
        set { _ubicacionSeleccionada = value; OnPropertyChanged(); if (value != null) CargarProductos(); }
    }

    public CatalogoViewModel(ProductoService productoService, UbicacionService ubicacionService, string tenantId)
    {
        _productoService = productoService;
        _ubicacionService = ubicacionService;
        _tenantId = tenantId;
        CargarUbicaciones();
    }

    private async void CargarUbicaciones()
    {
        var list = await _ubicacionService.GetUbicacionesTenantAsync(_tenantId);
        Ubicaciones.Clear();
        foreach (var u in list)
            Ubicaciones.Add(u);

        UbicacionSeleccionada = Ubicaciones.FirstOrDefault();
    }

    private async void CargarProductos()
    {
        if (UbicacionSeleccionada == null) return;
        var productos = await _productoService.GetProductosPorUbicacionAsync(UbicacionSeleccionada.Id);
        Productos.Clear();
        foreach (var p in productos)
            Productos.Add(p);
    }
}
