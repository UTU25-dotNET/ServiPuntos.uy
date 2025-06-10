using System.Text.Json.Serialization;

namespace ServiPuntos.Mobile.Models
{
    public class RegisterRequest
    {
        [JsonPropertyName("usuario")]
        public string Usuario { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }
    }
}
