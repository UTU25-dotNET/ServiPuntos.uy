using System;
using System.Text.Json.Serialization;

namespace ServiPuntos.Mobile.Models
{
    public class CanjeHistorialItem
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("fecha")]
        public DateTime Fecha { get; set; }

        [JsonPropertyName("producto")]
        public string Producto { get; set; }

        [JsonPropertyName("ubicacion")]
        public string Ubicacion { get; set; }

        [JsonPropertyName("puntos")]
        public int Puntos { get; set; }

        [JsonPropertyName("estado")]
        public string Estado { get; set; }
    }
}
