using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiPuntos.Core.Entities
{
    public class SaldoPuntos
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UsuarioId { get; set; }

        [Required]
        public int TenantId { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Saldo { get; set; }

        [Required]
        public DateTime UltimaActualizacion { get; set; }

        // Navegación
        public virtual Usuario Usuario { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}