using System;
using System.Collections.Generic;

namespace ServiPuntos.Core.NAFTA
{
    public class RespuestaNAFTA
    {
        public string IdMensajeReferencia { get; set; }
        public string Codigo { get; set; }  // "OK", "ERROR", etc.
        public string Mensaje { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public Dictionary<string, object> Datos { get; set; } = new Dictionary<string, object>();
    }
}