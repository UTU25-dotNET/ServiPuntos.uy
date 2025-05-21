using ServiPuntos.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiPuntos.Core.Entities
{
    public class Canje
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UsuarioId { get; set; }

        [Required]
        public Guid UbicacionId { get; set; }

        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public Guid ProductoCanjeableId { get; set; }

        [Required]
        public string CodigoQR { get; set; }

        [Required]
        public DateTime FechaGeneracion { get; set; }

        [Required]
        public DateTime FechaExpiracion { get; set; }

        public DateTime? FechaCanje { get; set; }

        [Required]
        public EstadoCanje Estado { get; set; }

        [Required]
        public int PuntosCanjeados { get; set; }

        // Navegación
        public virtual Usuario Usuario { get; set; }
        public virtual Ubicacion Ubicacion { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual ProductoCanjeable ProductoCanjeable { get; set; }
    }
}