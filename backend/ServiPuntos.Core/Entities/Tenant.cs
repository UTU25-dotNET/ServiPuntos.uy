
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServiPuntos.Core.Entities
{
    public class Tenant
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Nombre { get; set; } = null!;

        public string? LogoUrl { get; set; }
        public string? Color { get; set; }
        public decimal ValorPunto { get; set; }

        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; }

        public virtual ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
        public virtual ICollection<Ubicacion> Ubicaciones { get; set; } = new List<Ubicacion>();
    }
}
