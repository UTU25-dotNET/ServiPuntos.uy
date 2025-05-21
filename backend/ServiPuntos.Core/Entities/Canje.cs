using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiPuntos.Core.Entities
{
    public class Canje
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int UbicacionId { get; set; }

        [Required]
        public int TenantId { get; set; }

        [Required]
        public int ProductoCanjeableId { get; set; }

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
        [Column(TypeName = "decimal(18,2)")]
        public decimal PuntosCanjeados { get; set; }

        // Navegación
        public virtual Usuario Usuario { get; set; }
        public virtual Ubicacion Ubicacion { get; set; }
        public virtual Tenant Tenant { get; set; }
        public virtual ProductoCanjeable ProductoCanjeable { get; set; }
    }
}