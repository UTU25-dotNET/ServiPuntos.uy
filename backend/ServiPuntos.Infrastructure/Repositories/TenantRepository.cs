using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Interfaces;

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
            return await _dbContext.Tenants.FindAsync(id);
        }

        public async Task<IEnumerable<Tenant>> GetAllAsync()
        {
            return await _dbContext.Tenants.ToListAsync();
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

