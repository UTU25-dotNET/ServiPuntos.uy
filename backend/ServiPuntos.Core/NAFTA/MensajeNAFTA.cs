using System;
using System.Collections.Generic;

namespace ServiPuntos.Core.NAFTA
{
    public class MensajeNAFTA
    {
        public string Version { get; set; } = "1.0";
        public Guid IdMensaje { get; set; } = Guid.NewGuid();
        public string TipoMensaje { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Guid UbicacionId { get; set; }
        public Guid TenantId { get; set; }
        public Guid TerminalId { get; set; }
        public Dictionary<string, object> Datos { get; set; } = new Dictionary<string, object>();
    }
}