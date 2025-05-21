using System;
using System.Collections.Generic;

namespace ServiPuntos.Core.NAFTA
{
    public class TransaccionNAFTA
    {
        public Guid IdTransaccion { get; set; } = Guid.NewGuid();
        public Guid IdentificadorUsuario { get; set; }
        public DateTime FechaTransaccion { get; set; } = DateTime.UtcNow;
        public string TipoTransaccion { get; set; }
        public int Monto { get; set; }
        public string MonedaTransaccion { get; set; } = "UYU";
        public string MetodoPago { get; set; }
        public List<LineaTransaccionNAFTA> Productos { get; set; } = new List<LineaTransaccionNAFTA>();
        public Dictionary<string, object> DatosAdicionales { get; set; } = new Dictionary<string, object>();
    }
}