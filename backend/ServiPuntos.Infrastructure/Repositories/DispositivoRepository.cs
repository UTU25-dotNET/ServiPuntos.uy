using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.Data;

namespace ServiPuntos.Infrastructure.Repositories
{
    public class DispositivoRepository : IDispositivoRepository
    {
        private readonly ServiPuntosDbContext _context;
        public DispositivoRepository(ServiPuntosDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(DispositivoFcm dispositivo)
        {
            _context.DispositivosFcm.Add(dispositivo);
            return _context.SaveChangesAsync();
        }

        public Task<List<DispositivoFcm>> GetByUsuarioAsync(Guid usuarioId)
            => _context.DispositivosFcm.Where(d => d.UsuarioId == usuarioId).ToListAsync();

        public Task<List<DispositivoFcm>> GetByUsuariosAsync(IEnumerable<Guid> usuarioIds)
            => _context.DispositivosFcm.Where(d => usuarioIds.Contains(d.UsuarioId)).ToListAsync();
    }
}
