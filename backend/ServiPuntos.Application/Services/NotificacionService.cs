using FirebaseAdmin.Messaging;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Enums;
using ServiPuntos.Core.Interfaces;
using FirebaseAdmin.Messaging;

namespace ServiPuntos.Application.Services
{
    public class NotificacionService : INotificacionService
    {
        private readonly INotificacionRepository _repository;
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IFcmService _fcmService;

        public NotificacionService(INotificacionRepository repository, IUsuarioRepository usuarioRepository, IFcmService fcmService)
        {
            _repository = repository;
            _usuarioRepository = usuarioRepository;
            _fcmService = fcmService;
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
                
                if (!string.IsNullOrEmpty(u.TokenFcm))
                {
                    AndroidConfig androidConfig = new AndroidConfig
                    {
                        Priority = Priority.High,
                    };
                    await _fcmService.SendAsync(u.TokenFcm, notificacion.Titulo, notificacion.Mensaje, androidConfig);
                }
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