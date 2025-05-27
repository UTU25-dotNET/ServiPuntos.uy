using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.Data;

namespace ServiPuntos.Infrastructure.Repositories
{
    public class TenantRepository : ITenantRepository
    {
        private readonly ServiPuntosDbContext _dbContext;

        public TenantRepository(ServiPuntosDbContext context)
        {
            _dbContext = context;
        }

        public async Task<Tenant?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Tenants
                // incluir ubicaciones
                .Include(t => t.Ubicaciones)
                    // dentro de cada ubicación, incluir los productos locales
                    .ThenInclude(u => u.ProductosLocales)
                        // si quieres traer también datos del producto canjeable
                        .ThenInclude(pl => pl.ProductoCanjeable)
                .Include(t => t.Ubicaciones)
                    // e incluir las promociones si las necesitas
                    .ThenInclude(u => u.Promociones)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Tenant>> GetAllAsync()
        {
            return await _dbContext.Tenants
                // opcionalmente traer las ubicaciones aquí también
                .Include(t => t.Ubicaciones)
                .ToListAsync();
        }

        public async Task AddAsync(Tenant tenant)
        {
            await _dbContext.Tenants.AddAsync(tenant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tenant tenant)
        {
            _dbContext.Tenants.Update(tenant);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var tenant = await _dbContext.Tenants.FindAsync(id);
            if (tenant != null)
            {
                _dbContext.Tenants.Remove(tenant);
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
