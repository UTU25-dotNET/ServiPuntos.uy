using System;

namespace ServiPuntos.Mobile.Models;

public class TransaccionDto
{
    public Guid Id { get; set; }
    public DateTime Fecha { get; set; }
    public decimal Monto { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string? Ubicacion { get; set; }
    public int PuntosOtorgados { get; set; }
    public int PuntosUtilizados { get; set; }
    public string? Detalles { get; set; }
}
