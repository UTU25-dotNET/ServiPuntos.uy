using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ServiPuntos.Core.Entities
{
    public class Audiencia
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }

        [Required]
        [MaxLength(100)]
        public string NombreUnicoInterno { get; set; } // Para Usuario.SegmentoDinamicoId y URLs/APIs

        [Required]
        [MaxLength(250)]
        public string NombreDescriptivo { get; set; } // Para mostrar en la UI

        public string? Descripcion { get; set; }

        [Required]
        public int Prioridad { get; set; } // AÑADIDA: Crucial para desempate


        public DateTime FechaCreacion { get; set; }
        public DateTime FechaModificacion { get; set; } // AÑADIDA: Para rastrear cambios
        public bool Activa { get; set; }

        public Tenant Tenant { get; set; } // Propiedad de navegación
        public List<Promocion> Promociones { get; set; } = new();
        public List<ReglaAudiencia> Reglas { get; set; } = new();
        public Audiencia() {
            Id = Guid.NewGuid();
            FechaCreacion = DateTime.UtcNow;
            FechaModificacion = DateTime.UtcNow;
            Activa = true;
            Reglas = new List<ReglaAudiencia>();
        }

        // Constructor modificado
        public Audiencia(string nombreUnicoInterno, string nombreDescriptivo, string descripcion, Guid tenantId, int prioridad)
        {
            Id = Guid.NewGuid();
            NombreUnicoInterno = nombreUnicoInterno;
            NombreDescriptivo = nombreDescriptivo;
            Descripcion = descripcion;
            TenantId = tenantId;
            Prioridad = prioridad;
            FechaCreacion = DateTime.UtcNow;
            FechaModificacion = DateTime.UtcNow;
            Activa = true;
            Reglas = new List<ReglaAudiencia>();
        }
    }
}