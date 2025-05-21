using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiPuntos.Core.Entities
{
    public class Transaccion
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
        public DateTime FechaTransaccion { get; set; }

        [Required]
        public TipoTransaccion TipoTransaccion { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Monto { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PuntosOtorgados { get; set; }

        // Referencias opcionales para detalles adicionales
        public int? ProductoId { get; set; }

        public string Detalles { get; set; } // Para almacenar detalles adicionales en formato JSON

        public string ReferenciaExterna { get; set; } // Referencia del sistema POS

        // Navegación
        public virtual Usuario Usuario { get; set; }
        public virtual Ubicacion Ubicacion { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}