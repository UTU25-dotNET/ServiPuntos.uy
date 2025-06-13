using System;
using System.Collections.Generic;

namespace ServiPuntos.Mobile.Models
{
    public class MensajeNAFTA
    {
        public string Version { get; set; } = "1.0";
        public Guid IdMensaje { get; set; } = Guid.NewGuid();
        public int TipoMensaje { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Guid UbicacionId { get; set; }
        public Guid TenantId { get; set; }
        public Dictionary<string, object> Datos { get; set; } = new();
    }
}

public class RespuestaNAFTA
{
    public Guid IdMensajeReferencia { get; set; }
    public string Codigo { get; set; }
    public string Mensaje { get; set; }
    public Dictionary<string, object> Datos { get; set; } = new();

    public string CodigoQrBase64 { get; set; }
}
