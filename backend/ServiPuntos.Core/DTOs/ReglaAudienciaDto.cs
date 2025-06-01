using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.DTOs
{
    public class ReglaAudienciaDto
    {
        public Guid Id { get; set; }
        public string Propiedad { get; set; }
        public string Operador { get; set; }
        public string Valor { get; set; }
        public string OperadorLogicoConSiguiente { get; set; }
        public int OrdenEvaluacion { get; set; }

        // Mapeo a la entidad ReglaAudiencia
        public static ReglaAudiencia ToEntity(ReglaAudienciaDto dto)
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
        // Mapeo desde la entidad ReglaAudiencia
        public static ReglaAudienciaDto FromEntity(ReglaAudiencia regla)
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
    }
}
