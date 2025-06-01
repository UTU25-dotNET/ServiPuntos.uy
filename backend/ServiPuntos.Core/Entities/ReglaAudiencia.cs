// Namespace: ServiPuntos.Core.Entities
using System;
using System.ComponentModel.DataAnnotations;
using ServiPuntos.Core.DTOs;

namespace ServiPuntos.Core.Entities
{
    public class ReglaAudiencia
    {
        public Guid Id { get; set; } // O Guid
        public Guid AudienciaId { get; set; }

        [Required]
        [MaxLength(100)]
        public string Propiedad { get; set; } // Nombre de la propiedad del usuario a evaluar

        [Required]
        [MaxLength(50)]
        public string Operador { get; set; }  // Operador de comparación

        /// <summary>
        /// Valor de comparación. Se almacena como string en la BD.
        /// El motor de reglas lo interpretará y parseará según la Propiedad y Operador.
        /// </summary>
        [MaxLength(500)]
        public string Valor { get; set; }

        public string? OperadorLogicoConSiguiente { get; set; }
        public int OrdenEvaluacion { get; set; } // O simplemente Orden si es para evaluación

        public Audiencia Audiencia { get; set; }

        // Mapeo a la entidad ReglaAudienciaDto
        public static ReglaAudienciaDto ToDto(ReglaAudiencia regla)
        {
            return new ReglaAudienciaDto
            {
                Id = regla.Id,
                Propiedad = regla.Propiedad,
                Operador = regla.Operador,
                Valor = regla.Valor,
                OperadorLogicoConSiguiente = regla.OperadorLogicoConSiguiente,
                OrdenEvaluacion = regla.OrdenEvaluacion
            };
        }

        // Mapeo desde la entidad ReglaAudienciaDto
        public static ReglaAudiencia FromDto(ReglaAudienciaDto dto)
        {
            return new ReglaAudiencia
            {
                Id = dto.Id,
                Propiedad = dto.Propiedad,
                Operador = dto.Operador,
                Valor = dto.Valor,
                OperadorLogicoConSiguiente = dto.OperadorLogicoConSiguiente,
                OrdenEvaluacion = dto.OrdenEvaluacion
            };
        }
    }
}