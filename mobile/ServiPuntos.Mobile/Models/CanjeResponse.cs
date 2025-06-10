
using System;
using System.Text.Json.Serialization;

namespace ServiPuntos.Mobile.Models
{
    public class CanjeResponse
    {
        [JsonPropertyName("codigoQrBase64")]
        public string CodigoQrBase64 { get; set; }

        [JsonPropertyName("expiracion")]
        public DateTime Expiracion { get; set; }

        [JsonPropertyName("mensaje")]
        public string Mensaje { get; set; }
    }
}
