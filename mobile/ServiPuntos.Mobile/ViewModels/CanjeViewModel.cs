using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;

public class CanjeViewModel : BindableObject
{
    private readonly CanjeService _canjeService;
    public ProductoCanjeable Producto { get; set; }
    public Ubicacion Ubicacion { get; set; }
    public string TenantId { get; set; }
    public string UsuarioId { get; set; }

    private string _qrBase64;
    public string QrBase64
    {
        get => _qrBase64;
        set { _qrBase64 = value; OnPropertyChanged(); }
    }

    private string _mensaje;
    public string Mensaje
    {
        get => _mensaje;
        set { _mensaje = value; OnPropertyChanged(); }
    }

    public Command CanjearCommand { get; }

    public CanjeViewModel(CanjeService canjeService)
    {
        _canjeService = canjeService;
        CanjearCommand = new Command(async () => await CanjearAsync());
    }

    private async Task CanjearAsync()
    {
        var resp = await _canjeService.GenerarCanje(Producto.Id, Ubicacion.Id, TenantId, UsuarioId);
        QrBase64 = resp.CodigoQrBase64;
        Mensaje = resp.Mensaje;
    }
}
