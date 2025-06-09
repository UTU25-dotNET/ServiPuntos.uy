using System;
using System.Collections.Generic;

namespace ServiPuntos.Mobile.Models
{
    public class Usuario
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Email { get; set; }
        public int Ci { get; set; }
        public string Rol { get; set; }
        public string Telefono { get; set; }
        public string CiudadResidencia { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public int Puntos { get; set; }
        public bool VerificadoVEAI { get; set; }

    }
}
