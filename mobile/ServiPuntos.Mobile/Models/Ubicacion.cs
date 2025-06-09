using System.Text.Json.Serialization;

namespace ServiPuntos.Mobile.Models
{
    public class Ubicacion
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("direccion")]
        public string Direccion { get; set; }

        [JsonPropertyName("ciudad")]
        public string Ciudad { get; set; }

        [JsonPropertyName("departamento")]
        public string Departamento { get; set; }

        [JsonPropertyName("lavado")]
        public bool Lavado { get; set; }

        [JsonPropertyName("cambioDeAceite")]
        public bool CambioDeAceite { get; set; }

        [JsonPropertyName("cambioDeNeumaticos")]
        public bool CambioDeNeumaticos { get; set; }

        [JsonPropertyName("latitud")]
        public double Latitud { get; set; }

        [JsonPropertyName("longitud")]
        public double Longitud { get; set; }

        [JsonPropertyName("precioNaftaSuper")]
        public decimal PrecioNaftaSuper { get; set; }

        [JsonPropertyName("precioNaftaPremium")]
        public decimal PrecioNaftaPremium { get; set; }

        [JsonPropertyName("precioDiesel")]
        public decimal PrecioDiesel { get; set; }
    }
}
