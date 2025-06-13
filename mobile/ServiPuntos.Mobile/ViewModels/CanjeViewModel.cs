using ServiPuntos.Mobile.Models;
using ServiPuntos.Mobile.Services;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

public class CanjeViewModel : BindableObject
{
    private readonly ICanjeService _canjeService;

    public ProductoCanjeable Producto { get; set; } = new ProductoCanjeable();
    public Ubicacion Ubicacion { get; set; } = new Ubicacion();
    public string TenantId { get; set; } = string.Empty;
    public string UsuarioId { get; set; } = string.Empty;

    private string _qrBase64 = string.Empty;
    public string QrBase64
    {
        get => _qrBase64;
        set { _qrBase64 = value; OnPropertyChanged(); }
    }

    private string _mensaje = string.Empty;
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
        if (producto == null || string.IsNullOrWhiteSpace(TenantId) || string.IsNullOrWhiteSpace(UsuarioId) || Ubicacion == null)
        {
            Mensaje = "Faltan datos para realizar el canje.";
            return;
        }

        var req = new MensajeNAFTA
        {
            TenantId = Guid.Parse(TenantId),
            UbicacionId = Guid.Parse(Ubicacion.Id),
            Datos = new Dictionary<string, object>
            {
                { "productoCanjeableId", producto.Id },
                { "usuarioId", Guid.Parse(UsuarioId) }
            }
        };

        try
        {
            var resp = await _canjeService.GenerarCanjeAsync(req);
            QrBase64 = resp.CodigoQrBase64 ?? string.Empty;
            Mensaje = resp.Mensaje ?? "¡Canje realizado!";
        }
        catch (Exception ex)
        {
            Mensaje = "Hubo un error al canjear: " + ex.Message;
            QrBase64 = string.Empty;
        }
    }
}
