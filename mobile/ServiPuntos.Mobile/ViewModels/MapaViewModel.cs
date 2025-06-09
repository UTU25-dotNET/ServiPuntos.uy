using System.Collections.ObjectModel;
using System.Linq;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;
using Microsoft.Maui.Controls;
using System.Windows.Input;

public class MapaViewModel : BindableObject
{
    private readonly UbicacionService _ubicacionService;
    private readonly string _tenantId;

    public ObservableCollection<Ubicacion> Ubicaciones { get; } = new();
    public ObservableCollection<string> Ciudades { get; } = new();

    private string _ciudadSeleccionada;
    public string CiudadSeleccionada
    {
        get => _ciudadSeleccionada;
        set { _ciudadSeleccionada = value; OnPropertyChanged(); AplicarFiltros(); }
    }

    private bool _filtrarLavado;
    public bool FiltrarLavado
    {
        get => _filtrarLavado;
        set { _filtrarLavado = value; OnPropertyChanged(); AplicarFiltros(); }
    }

    private bool _filtrarCambioAceite;
    public bool FiltrarCambioAceite
    {
        get => _filtrarCambioAceite;
        set { _filtrarCambioAceite = value; OnPropertyChanged(); AplicarFiltros(); }
    }

    private decimal _precioNaftaSuperMax = 200;
    public decimal PrecioNaftaSuperMax
    {
        get => _precioNaftaSuperMax;
        set { _precioNaftaSuperMax = value; OnPropertyChanged(); AplicarFiltros(); }
    }

    private List<Ubicacion> _todasLasUbicaciones;

    public MapaViewModel(UbicacionService ubicacionService, string tenantId)
    {
        _ubicacionService = ubicacionService;
        _tenantId = tenantId;
        CargarUbicacionesCommand = new Command(async () => await CargarUbicacionesAsync());
        CargarUbicacionesCommand.Execute(null);
    }

    public ICommand CargarUbicacionesCommand { get; }

    private async Task CargarUbicacionesAsync()
    {
        Ubicaciones.Clear();
        _todasLasUbicaciones = await _ubicacionService.GetUbicacionesTenantAsync(_tenantId) ?? new List<Ubicacion>();
        var ciudadesUnicas = _todasLasUbicaciones
            .Select(u => u.Ciudad)
            .Where(c => !string.IsNullOrEmpty(c))
            .Distinct()
            .OrderBy(c => c)
            .ToList();
        Ciudades.Clear();
        foreach (var ciudad in ciudadesUnicas)
            Ciudades.Add(ciudad);

        CiudadSeleccionada = Ciudades.FirstOrDefault();
        AplicarFiltros();
    }

    private void AplicarFiltros()
    {
        if (_todasLasUbicaciones == null)
            return;

        var filtradas = _todasLasUbicaciones
            .Where(u => string.IsNullOrEmpty(CiudadSeleccionada) || u.Ciudad == CiudadSeleccionada)
            .Where(u => !FiltrarLavado || u.LavadoDeAuto || u.Lavado)
            .Where(u => !FiltrarCambioAceite || u.CambioDeAceite)
            .Where(u => u.PrecioNaftaSuper <= PrecioNaftaSuperMax)
            .ToList();

        Ubicaciones.Clear();
        foreach (var u in filtradas)
            Ubicaciones.Add(u);
    }
}
