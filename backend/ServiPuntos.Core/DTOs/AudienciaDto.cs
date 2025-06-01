namespace ServiPuntos.Core.DTOs
{
    public class AudienciaDto
    {
        public Guid Id { get; set; } // Usar Guid.Empty para nuevas audiencias
        public string NombreUnicoInterno { get; set; }
        public string NombreDescriptivo { get; set; }
        public string Descripcion { get; set; }
        public int Prioridad { get; set; }
        public bool Activa { get; set; }
        public List<ReglaAudienciaDto> Reglas { get; set; } = new List<ReglaAudienciaDto>();
    }
}
