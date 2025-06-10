
using System.Text.Json.Serialization;

namespace ServiPuntos.Mobile.Models
{
    public class UserSession
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("rol")]
        public string Rol { get; set; }

        [JsonIgnore]
        public bool IsAuthenticated => !string.IsNullOrEmpty(Token);
    }
}
