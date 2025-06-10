
using System.Text.Json.Serialization;

namespace ServiPuntos.Mobile.Models
{
    public class LoginResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("rol")]
        public string Rol { get; set; }
    }
}
