using System;
using System.Collections.Generic;

namespace ServiPuntos.Core.NAFTA
{
    public class MensajeNAFTA
    {
        public string Version { get; set; } = "1.0";
        public string IdMensaje { get; set; } = Guid.NewGuid().ToString();
        public string TipoMensaje { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string UbicacionId { get; set; }
        public string TenantId { get; set; }
        public string TerminalId { get; set; }
        public Dictionary<string, object> Datos { get; set; } = new Dictionary<string, object>();
    }
}