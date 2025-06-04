using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.DTOs
{
    public class AudienciaDto
    {
        public Guid Id { get; set; } // Usar Guid.Empty para nuevas audiencias
        public string NombreUnicoInterno { get; set; }
        public string NombreDescriptivo { get; set; }
        public string Descripcion { get; set; }
        public int CantidadUsuarios { get; set; } = 0; // AÑADIDA: Para mostrar en la UI
        public int Prioridad { get; set; }
        public bool Activa { get; set; }
        public List<ReglaAudienciaDto> Reglas { get; set; } = new List<ReglaAudienciaDto>();

        // Constructor
        public AudienciaDto()
        {
            Id = Guid.Empty; // Inicializar con Guid.Empty para nuevas audiencias
            NombreUnicoInterno = string.Empty;
            NombreDescriptivo = string.Empty;
            Descripcion = string.Empty;
            CantidadUsuarios = 0; // Inicializar a 0
            Prioridad = 0; // Inicializar a 0
            Activa = true; // Por defecto, una audiencia está activa
        }
        public AudienciaDto(Guid id, string nombreUnicoInterno, string nombreDescriptivo, string descripcion, int cantidadUsuarios, int prioridad, bool activa)
        {
            Id = id;
            NombreUnicoInterno = nombreUnicoInterno;
            NombreDescriptivo = nombreDescriptivo;
            Descripcion = descripcion;
            CantidadUsuarios = cantidadUsuarios;
            Prioridad = prioridad;
            Activa = activa;
        }

        // MÉTODOS DE MAPEO DENTRO DEL DTO

        /// <summary>
        /// Convierte el DTO a entidad (método estático)
        /// </summary>
        public static Audiencia ToEntity(AudienciaDto dto, Guid tenantId)
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
        /// Crea un DTO desde una entidad (método estático)
        /// </summary>
        public static AudienciaDto FromEntity(Audiencia audiencia, int cantidadUsuarios)
        {
            return new AudienciaDto
            {
                Id = audiencia.Id,
                NombreUnicoInterno = audiencia.NombreUnicoInterno,
                NombreDescriptivo = audiencia.NombreDescriptivo,
                Descripcion = audiencia.Descripcion ?? string.Empty,
                CantidadUsuarios = cantidadUsuarios,
                Prioridad = audiencia.Prioridad,
                Activa = audiencia.Activa,
                Reglas = audiencia.Reglas?.Select(r => ReglaAudienciaDto.FromEntity(r)).ToList() ?? new List<ReglaAudienciaDto>()
            };
        }

        /// <summary>
        /// Convierte el DTO actual a entidad
        /// </summary>
        public Audiencia ToEntity(Guid tenantId)
        {
            return ToEntity(this, tenantId);
        }

        /// <summary>
        /// Actualiza una entidad existente con los datos del DTO
        /// </summary>
        public void UpdateEntity(Audiencia audiencia)
        {
            audiencia.NombreUnicoInterno = this.NombreUnicoInterno;
            audiencia.NombreDescriptivo = this.NombreDescriptivo;
            audiencia.Descripcion = this.Descripcion;
            audiencia.Prioridad = this.Prioridad;
            audiencia.Activa = this.Activa;
            audiencia.FechaModificacion = DateTime.UtcNow;
        }
    }
}
