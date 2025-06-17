using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.Data;

namespace ServiPuntos.Infrastructure.Repositories
{
    public class PromocionRepository : IPromocionRepository
    {
        private readonly ServiPuntosDbContext _dbContext;
        public PromocionRepository(ServiPuntosDbContext context)
        {
            _dbContext = context;
        }

        public Task<Promocion?> GetAsync(Guid id) =>
            _dbContext.Promociones
                .Include(p => p.Ubicaciones)
                .Include(p => p.Productos)!.ThenInclude(pp => pp.ProductoCanjeable)
                .FirstOrDefaultAsync(p => p.Id == id);

        public async Task<IEnumerable<Promocion>> GetAllAsync() =>
            await _dbContext.Promociones
                .Include(p => p.Ubicaciones)
                .Include(p => p.Productos)!.ThenInclude(pp => pp.ProductoCanjeable)
                .ToListAsync();

        public async Task<IEnumerable<Promocion>> GetAllByTenantAsync(Guid tenantId) =>
            await _dbContext.Promociones
                .Include(p => p.Ubicaciones)
                .Include(p => p.Productos)!.ThenInclude(pp => pp.ProductoCanjeable)
                .Where(p => p.TenantId == tenantId)
                .ToListAsync();

        public Task AddAsync(Promocion promocion)
        {
            _dbContext.Promociones.Add(promocion);
            return _dbContext.SaveChangesAsync();
        }

        public Task UpdateAsync(Promocion promocion)
        {
            _dbContext.Promociones.Update(promocion);
            return _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var promo = await _dbContext.Promociones.FirstOrDefaultAsync(p => p.Id == id);
            if (promo != null)
            {
                _dbContext.Promociones.Remove(promo);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}