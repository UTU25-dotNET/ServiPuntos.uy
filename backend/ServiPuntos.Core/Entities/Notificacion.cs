namespace ServiPuntos.Core.Entities
{
    public class Notificacion
    {
        public Guid Id { get; set; }
        public Guid TenantId { get; set; }
        public Guid? AudienciaId { get; set; }
        public string Titulo { get; set; } = string.Empty;
        public string Mensaje { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }

        public Audiencia? Audiencia { get; set; }
        public List<NotificacionUsuario>? Destinatarios { get; set; }
    }
}