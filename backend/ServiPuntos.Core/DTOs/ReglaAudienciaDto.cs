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
    }
}
