namespace ServiPuntos.Core.DTOs
{
    public class CrearAudienciaDto
    {
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public List<ReglaAudienciaDto> Reglas { get; set; } = new();
    }
}
