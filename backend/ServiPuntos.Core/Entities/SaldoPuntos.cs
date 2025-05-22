using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ServiPuntos.Core.Entities
{
    public class SaldoPuntos
    {
        [Key]
        public Guid Id { get; set; }

        [Required]
        public Guid UsuarioId { get; set; }

        [Required]
        public Guid TenantId { get; set; }

        [Required]
        public int Saldo { get; set; }

        [Required]
        public DateTime UltimaActualizacion { get; set; }

        // Navegación
        public virtual Usuario Usuario { get; set; }
        public virtual Tenant Tenant { get; set; }
    }
}