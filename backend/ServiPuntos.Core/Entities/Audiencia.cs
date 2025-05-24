using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ServiPuntos.Core.DTOs;

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
        public Audiencia(string nombreUnicoInterno, string nombreDescriptivo, string? descripcion, Guid tenantId, int prioridad)
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

        // MÉTODOS DE MAPEO DENTRO DE LA ENTIDAD

        /// <summary>
        /// Convierte la entidad a DTO
        /// </summary>
        public AudienciaDto ToDto(int cantidadUsuarios)
        {
            return new AudienciaDto
            {
                Id = this.Id,
                NombreUnicoInterno = this.NombreUnicoInterno,
                NombreDescriptivo = this.NombreDescriptivo,
                Descripcion = this.Descripcion ?? string.Empty,
                CantidadUsuarios = cantidadUsuarios,
                Prioridad = this.Prioridad,
                Activa = this.Activa,
                Reglas = this.Reglas?.Select(r => ReglaAudiencia.ToDto(r)).ToList() ?? new List<ReglaAudienciaDto>()
            };
        }

        /// <summary>
        /// Crea una entidad desde un DTO (método estático)
        /// </summary>
        public static Audiencia FromDto(AudienciaDto dto, Guid tenantId)
        {
            return new Audiencia(
                dto.NombreUnicoInterno,
                dto.NombreDescriptivo,
                dto.Descripcion,
                tenantId,
                dto.Prioridad
            );
        }

        /// <summary>
        /// Actualiza la entidad desde un DTO
        /// </summary>
        public void UpdateFromDto(AudienciaDto dto)
        {
            this.NombreUnicoInterno = dto.NombreUnicoInterno;
            this.NombreDescriptivo = dto.NombreDescriptivo;
            this.Descripcion = dto.Descripcion;
            this.Prioridad = dto.Prioridad;
            this.Activa = dto.Activa;
            this.FechaModificacion = DateTime.UtcNow;

            // Si necesitas actualizar las reglas también
            // this.Reglas = dto.Reglas?.Select(r => ReglaAudiencia.FromDto(r)).ToList() ?? new List<ReglaAudiencia>();
        }
    }
}