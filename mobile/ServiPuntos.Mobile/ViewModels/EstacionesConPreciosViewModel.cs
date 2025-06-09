using System.Collections.ObjectModel;
using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;
using Microsoft.Maui.Controls;
using System.Windows.Input;
using System.Linq;

public class EstacionesConPreciosViewModel : BindableObject
{
    private readonly UbicacionService _ubicacionService;
    private readonly string _tenantId;

    public ObservableCollection<Ubicacion> Estaciones { get; } = new();

    public ICommand CargarCommand { get; }

    public EstacionesConPreciosViewModel(UbicacionService ubicacionService, string tenantId)
    {
        _ubicacionService = ubicacionService;
        _tenantId = tenantId;
        CargarCommand = new Command(async () => await CargarEstacionesAsync());
        CargarCommand.Execute(null);
    }

    private async System.Threading.Tasks.Task CargarEstacionesAsync()
    {
        Estaciones.Clear();
        var list = await _ubicacionService.GetUbicacionesTenantAsync(_tenantId) ?? new System.Collections.Generic.List<Ubicacion>();
        foreach (var e in list.OrderBy(x => x.Nombre))
            Estaciones.Add(e);
    }
}
