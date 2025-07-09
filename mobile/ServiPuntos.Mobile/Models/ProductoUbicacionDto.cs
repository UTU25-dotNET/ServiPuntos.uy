using System;
using System.Text.Json.Serialization;

namespace ServiPuntos.Mobile.Models
{
    public class ProductoUbicacionDto
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("ubicacionId")]
        public Guid UbicacionId { get; set; }

        [JsonPropertyName("categoria")]
        public string Categoria { get; set; } = string.Empty;

        [JsonPropertyName("productoCanjeableId")]
        public Guid ProductoCanjeableId { get; set; }

        [JsonPropertyName("precio")]
        public double Precio { get; set; }

        [JsonPropertyName("stockDisponible")]
        public int StockDisponible { get; set; }

        [JsonPropertyName("activo")]
        public bool Activo { get; set; }

        [JsonPropertyName("productoCanjeable")]
        public ProductoCanjeableInfo? ProductoCanjeable { get; set; }

        public class ProductoCanjeableInfo
        {
            [JsonPropertyName("id")]
            public Guid Id { get; set; }

            [JsonPropertyName("nombre")]
            public string Nombre { get; set; } = string.Empty;

            [JsonPropertyName("descripcion")]
            public string Descripcion { get; set; } = string.Empty;

            [JsonPropertyName("costoEnPuntos")]
            public int CostoEnPuntos { get; set; }

            [JsonPropertyName("fotoUrl")]
            public string? FotoUrl { get; set; }
        }
    }
}
