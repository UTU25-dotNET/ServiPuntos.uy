using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface INotificacionRepository
    {
        Task AddAsync(Notificacion notificacion);
        Task AddUsuarioAsync(NotificacionUsuario notificacionUsuario);
        Task<IEnumerable<NotificacionUsuario>> GetByUsuarioAsync(Guid usuarioId);
        Task MarkAsLeidaAsync(Guid notificacionUsuarioId);
        Task DeleteUsuarioAsync(Guid notificacionUsuarioId);
        Task DeleteAllByUsuarioAsync(Guid usuarioId);
        /// <summary>
        /// Establece en null la referencia de Audiencia en todas las notificaciones asociadas.
        /// </summary>
        Task ClearAudienciaAsync(Guid audienciaId);
    }
}