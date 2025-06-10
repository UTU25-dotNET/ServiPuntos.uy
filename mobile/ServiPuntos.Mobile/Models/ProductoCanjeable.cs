
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ServiPuntos.Mobile.Models
{
    public class ProductoCanjeable
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("descripcion")]
        public string Descripcion { get; set; }

        [JsonPropertyName("costoEnPuntos")]
        public int CostoEnPuntos { get; set; }

        [JsonPropertyName("disponibilidadesPorUbicacion")]
        public List<DisponibilidadPorUbicacion> DisponibilidadesPorUbicacion { get; set; }
    }

    public class DisponibilidadPorUbicacion
    {
        [JsonPropertyName("ubicacionId")]
        public string UbicacionId { get; set; }

        [JsonPropertyName("stockDisponible")]
        public int StockDisponible { get; set; }

        [JsonPropertyName("activo")]
        public bool Activo { get; set; }
    }
}
