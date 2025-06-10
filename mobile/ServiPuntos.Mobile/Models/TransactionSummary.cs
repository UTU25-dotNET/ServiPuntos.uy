
using System;
using System.Text.Json.Serialization;

namespace ServiPuntos.Mobile.Models
{
    public class TransactionSummary
    {
        [JsonPropertyName("fecha")]
        public DateTime Fecha { get; set; }

        [JsonPropertyName("descripcion")]
        public string Descripcion { get; set; }

        [JsonPropertyName("puntos")]
        public int Puntos { get; set; }

        [JsonPropertyName("tipo")]
        public string Tipo { get; set; }
    }
}
