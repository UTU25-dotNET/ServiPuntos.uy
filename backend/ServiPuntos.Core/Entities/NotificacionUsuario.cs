namespace ServiPuntos.Core.Entities
{
    public class NotificacionUsuario
    {
        public Guid Id { get; set; }
        public Guid NotificacionId { get; set; }
        public Guid UsuarioId { get; set; }
        public bool Leida { get; set; }
        public DateTime? FechaLeida { get; set; }

        public Notificacion? Notificacion { get; set; }
        public Usuario? Usuario { get; set; }
    }
}