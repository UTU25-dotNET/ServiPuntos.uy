using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface INotificacionService
    {
        Task<IEnumerable<NotificacionUsuario>> ObtenerPorUsuarioAsync(Guid usuarioId);
        Task CrearNotificacionAsync(Notificacion notificacion, Guid? audienciaId);
        Task MarcarComoLeidaAsync(Guid notificacionUsuarioId);
        Task EliminarParaUsuarioAsync(Guid notificacionUsuarioId);
        Task DeleteAllByUsuarioAsync(Guid usuarioId);
    }
}