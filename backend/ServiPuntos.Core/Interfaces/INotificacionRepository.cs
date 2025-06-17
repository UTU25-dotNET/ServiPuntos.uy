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
    }
}