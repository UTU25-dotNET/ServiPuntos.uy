using System;
using System.Collections.Generic;
using System.Text.Json;

namespace ServiPuntos.Mobile.Models
{
    public class RespuestaNAFTA
    {
        public Guid IdMensajeReferencia { get; set; }

        public string Codigo { get; set; } = string.Empty;

        public string Mensaje { get; set; } = string.Empty;

        public DateTime Timestamp { get; set; }

        public Dictionary<string, JsonElement> Datos { get; set; } = new();
    }
}
