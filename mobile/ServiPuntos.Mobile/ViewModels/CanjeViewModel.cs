using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;

public class CanjeViewModel : BindableObject
{
    private readonly ICanjeService _canjeService;
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

    public ICommand CanjearCommand { get; }

    public CanjeViewModel(ICanjeService canjeService)
    {
        _canjeService = canjeService;
        CanjearCommand = new Command<ProductoCanjeable>(async (producto) => await CanjearAsync(producto));
    }

    private async Task CanjearAsync(ProductoCanjeable producto)
    {
        if (producto == null) return;

        var req = new CanjeRequest
        {
            ProductoId = producto.Id,
            Cantidad = 1
        };

        var resp = await _canjeService.GenerarCanjeAsync(req);
        QrBase64 = resp.CodigoQrBase64;
        Mensaje = resp.Mensaje;
    }
}
