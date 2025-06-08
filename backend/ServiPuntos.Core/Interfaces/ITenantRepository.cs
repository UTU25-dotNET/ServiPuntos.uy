using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface ITenantRepository
    {
        Task<Tenant?> GetByIdAsync(Guid id);
        Task<IEnumerable<Tenant>> GetAllAsync();
        Task AddAsync(Tenant tenant);
        Task UpdateAsync(Tenant tenant);
        Task DeleteAsync(Guid id);
    }
}
