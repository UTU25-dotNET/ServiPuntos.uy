using System;

namespace ServiPuntos.Core.NAFTA
{
    public class CanjeNAFTA
    {
        public string CodigoQR { get; set; }
        public Guid ProductoId { get; set; }
        public Guid UsuarioId { get; set; }
        public Guid? UbicacionId { get; set; }
        public DateTime FechaCanje { get; set; } = DateTime.UtcNow;
    }
}