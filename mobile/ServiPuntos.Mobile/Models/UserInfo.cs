
using System.Text.Json.Serialization;

namespace ServiPuntos.Mobile.Models
{
    public class UserInfo
    {
        [JsonPropertyName("userId")]
        public string UserId { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; } = string.Empty;
    }
}
