
using System.Text.Json.Serialization;

namespace ServiPuntos.Mobile.Models
{
    /// <summary>
    /// Representa la petición para generar un nuevo canje.
    /// </summary>
    public class CanjeRequest
    {
        [JsonPropertyName("productoId")]
        public string ProductoId { get; set; } = string.Empty;

        [JsonPropertyName("cantidad")]
        public int Cantidad { get; set; }
    }
}
