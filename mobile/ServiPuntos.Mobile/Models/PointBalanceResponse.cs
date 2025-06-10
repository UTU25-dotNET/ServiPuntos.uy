using System.Text.Json.Serialization;

namespace ServiPuntos.Mobile.Models
{
    public class PointBalanceResponse
    {
        [JsonPropertyName("saldo")]
        public int Saldo { get; set; }

        [JsonPropertyName("puntosAcumuladosMes")]
        public int PuntosAcumuladosMes { get; set; }
    }
}
