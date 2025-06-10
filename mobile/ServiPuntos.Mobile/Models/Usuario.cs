
using System;
using System.Text.Json.Serialization;

namespace ServiPuntos.Mobile.Models
{
    public class Usuario
    {
        [JsonPropertyName("id")]
        public Guid Id { get; set; }

        [JsonPropertyName("tenantId")]
        public Guid TenantId { get; set; }

        [JsonPropertyName("nombre")]
        public string Nombre { get; set; }

        [JsonPropertyName("apellido")]
        public string Apellido { get; set; }

        [JsonPropertyName("email")]
        public string Email { get; set; }

        [JsonPropertyName("ci")]
        public int Ci { get; set; }

        [JsonPropertyName("rol")]
        public string Rol { get; set; }

        [JsonPropertyName("telefono")]
        public string Telefono { get; set; }

        [JsonPropertyName("ciudadResidencia")]
        public string CiudadResidencia { get; set; }

        [JsonPropertyName("fechaNacimiento")]
        public DateTime? FechaNacimiento { get; set; }

        [JsonPropertyName("puntos")]
        public int Puntos { get; set; }

        [JsonPropertyName("verificadoVEAI")]
        public bool VerificadoVEAI { get; set; }
    }
}
