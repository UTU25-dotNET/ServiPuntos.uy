using System;

namespace ServiPuntos.Core.NAFTA
{
    public class CanjeNAFTA
    {
        public string CodigoQR { get; set; }
        public string IdProducto { get; set; }
        public string IdentificadorUsuario { get; set; }
        public string UbicacionId { get; set; }
        public DateTime FechaCanje { get; set; } = DateTime.UtcNow;
    }
}