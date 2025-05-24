using ServiPuntos.Core.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiPuntos.Core.Entities
{
    public class Transaccion
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
        public DateTime FechaTransaccion { get; set; }

        [Required]
        public TipoTransaccion TipoTransaccion { get; set; }

        [Required]
        public decimal Monto { get; set; }

        [Required]
        public int PuntosOtorgados { get; set; }

        // Referencias opcionales para detalles adicionales
        public int? ProductoId { get; set; }

        public string Detalles { get; set; } // Para almacenar detalles adicionales en formato JSON

        public Guid ReferenciaExterna { get; set; } // Referencia del sistema POS

        // Navegación
        public virtual Usuario Usuario { get; set; }
        public virtual Ubicacion Ubicacion { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}