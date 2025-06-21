using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Entities;
using ServiPuntos.Infrastructure.Data;
using ServiPuntos.Core.Interfaces;

namespace ServiPuntos.Infrastructure.Repositories
{
    public class UbicacionRepository : IUbicacionRepository
    {
        private readonly ServiPuntosDbContext _dbContext;

        public UbicacionRepository(ServiPuntosDbContext context)
        {
            _dbContext = context;
        }
        public Task<Ubicacion?> GetAsync(Guid idUbicacion)
            => _dbContext.Ubicaciones
                .FirstOrDefaultAsync(u => u.Id == idUbicacion);
        public Task<Ubicacion?> GetAsync(string nombre)
            => _dbContext.Ubicaciones
                .FirstOrDefaultAsync(u => u.Nombre == nombre);
        public Task<Ubicacion?> GetAsync(Guid tenantId, Guid idUbicacion)
            => _dbContext.Ubicaciones
                .FirstOrDefaultAsync(u => u.TenantId == tenantId & u.Id == idUbicacion);
        public async Task<IEnumerable<Ubicacion>> GetAllAsync()
            => await _dbContext.Ubicaciones
                .ToListAsync();
        public async Task<IEnumerable<Ubicacion>> GetAllAsync(Guid tenantId)
            => await _dbContext.Ubicaciones
                .Where(u => u.TenantId == tenantId)
                .ToListAsync();
        public Task AddAsync(Ubicacion ubicacion)
        {
            _dbContext.Ubicaciones.Add(ubicacion);
            return _dbContext.SaveChangesAsync();
        }
        public Task AddAsync(Guid tenantId, Ubicacion ubicacion)
        {
            ubicacion.TenantId = tenantId;
            _dbContext.Ubicaciones.Add(ubicacion);
            return _dbContext.SaveChangesAsync();
        }

        public Task UpdateAsync(Ubicacion ubicacion)
        {
            _dbContext.Ubicaciones.Update(ubicacion);
            return _dbContext.SaveChangesAsync();
        }
        public Task UpdateAsync(Guid tenantId, Ubicacion ubicacion)
        {
            ubicacion.TenantId = tenantId;
            _dbContext.Ubicaciones.Update(ubicacion);
            return _dbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid idUbicacion)
        {
            var ubicacion = await _dbContext.Ubicaciones
                .FirstOrDefaultAsync(u => u.Id == idUbicacion);
            if (ubicacion != null)
            {
                _dbContext.Ubicaciones.Remove(ubicacion);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task DeleteAsync(Guid tenantId, Guid idUbicacion)
        {
            var ubicacion = await _dbContext.Ubicaciones
                .FirstOrDefaultAsync(u => u.TenantId == tenantId && u.Id == idUbicacion);

            if (ubicacion != null)
            {
                // Eliminar canjes asociados
                var canjes = await _dbContext.Canjes
                    .Where(c => c.UbicacionId == ubicacion.Id)
                    .ToListAsync();
                if (canjes.Count > 0)
                {
                    _dbContext.Canjes.RemoveRange(canjes);
                }

                // Eliminar transacciones asociadas
                var transacciones = await _dbContext.Transacciones
                    .Where(t => t.UbicacionId == ubicacion.Id)
                    .ToListAsync();
                if (transacciones.Count > 0)
                {
                    _dbContext.Transacciones.RemoveRange(transacciones);
                }

                // Desasociar a los usuarios
                var usuarios = await _dbContext.Usuarios
                    .Where(u => u.UbicacionId == ubicacion.Id)
                    .ToListAsync();
                foreach (var usuario in usuarios)
                {
                    usuario.UbicacionId = null;
                }

                _dbContext.Ubicaciones.Remove(ubicacion);
                await _dbContext.SaveChangesAsync();
            }
        }

    }
}
