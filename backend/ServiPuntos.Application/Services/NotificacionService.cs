using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Enums;
using ServiPuntos.Core.Interfaces;

namespace ServiPuntos.Application.Services
{
    public class NotificacionService : INotificacionService
    {
        private readonly INotificacionRepository _repository;
        private readonly IUsuarioRepository _usuarioRepository;

        public NotificacionService(INotificacionRepository repository, IUsuarioRepository usuarioRepository)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
        }

        public Task<IEnumerable<NotificacionUsuario>> ObtenerPorUsuarioAsync(Guid usuarioId)
            => _repository.GetByUsuarioAsync(usuarioId);

        public async Task CrearNotificacionAsync(Notificacion notificacion, Guid? audienciaId)
        {
            await _repository.AddAsync(notificacion);
            var usuarios = await _usuarioRepository.GetAllByTenantAsync(notificacion.TenantId);
            IEnumerable<Usuario> destinatarios;
            if (audienciaId.HasValue)
            {
                destinatarios = usuarios.Where(u => u.Rol == RolUsuario.UsuarioFinal && u.SegmentoDinamicoId == audienciaId);
            }
            else
            {
                destinatarios = usuarios.Where(u => u.Rol == RolUsuario.UsuarioFinal);
            }

            foreach (var u in destinatarios)
            {
                var nu = new NotificacionUsuario
                {
                    Id = Guid.NewGuid(),
                    NotificacionId = notificacion.Id,
                    UsuarioId = u.Id,
                    Leida = false
                };
                await _repository.AddUsuarioAsync(nu);
            }
        }

        public Task MarcarComoLeidaAsync(Guid notificacionUsuarioId)
            => _repository.MarkAsLeidaAsync(notificacionUsuarioId);

        public Task EliminarParaUsuarioAsync(Guid notificacionUsuarioId)
            => _repository.DeleteUsuarioAsync(notificacionUsuarioId);

        public Task EliminarTodasParaUsuarioAsync(Guid usuarioId)
            => _repository.DeleteAllByUsuarioAsync(usuarioId);

        public Task DeleteAllByUsuarioAsync(Guid usuarioId)
            => _repository.DeleteAllByUsuarioAsync(usuarioId);
    }
}