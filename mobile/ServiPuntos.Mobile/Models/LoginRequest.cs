
using System.Text.Json.Serialization;

namespace ServiPuntos.Mobile.Models
{
    public class LoginRequest
    {
        [JsonPropertyName("usuario")]
        public string Usuario { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}
