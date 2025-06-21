using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.Data;

namespace ServiPuntos.Infrastructure.Repositories
{
    public class NotificacionRepository : INotificacionRepository
    {
        private readonly ServiPuntosDbContext _context;

        public NotificacionRepository(ServiPuntosDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(Notificacion notificacion)
        {
            _context.Notificaciones.Add(notificacion);
            return _context.SaveChangesAsync();
        }

        public Task AddUsuarioAsync(NotificacionUsuario notificacionUsuario)
        {
            _context.NotificacionUsuarios.Add(notificacionUsuario);
            return _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<NotificacionUsuario>> GetByUsuarioAsync(Guid usuarioId)
        {
            return await _context.NotificacionUsuarios
                .Include(nu => nu.Notificacion)
                .Where(nu => nu.UsuarioId == usuarioId)
                .OrderByDescending(nu => nu.Notificacion!.FechaCreacion)
                .ToListAsync();
        }

        public async Task MarkAsLeidaAsync(Guid notificacionUsuarioId)
        {
            var nu = await _context.NotificacionUsuarios.FirstOrDefaultAsync(n => n.Id == notificacionUsuarioId);
            if (nu != null)
            {
                nu.Leida = true;
                nu.FechaLeida = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeleteUsuarioAsync(Guid notificacionUsuarioId)
        {
            var nu = await _context.NotificacionUsuarios.FirstOrDefaultAsync(n => n.Id == notificacionUsuarioId);
            if (nu != null)
            {
                _context.NotificacionUsuarios.Remove(nu);
                await _context.SaveChangesAsync();
            }
        }
        public async Task DeleteAllByUsuarioAsync(Guid usuarioId)
        {
            var items = _context.NotificacionUsuarios.Where(nu => nu.UsuarioId == usuarioId);
            _context.NotificacionUsuarios.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task ClearAudienciaAsync(Guid audienciaId)
        {
            var notificaciones = _context.Notificaciones.Where(n => n.AudienciaId == audienciaId);
            await notificaciones.ForEachAsync(n => n.AudienciaId = null);
            await _context.SaveChangesAsync();
        }
    }
}