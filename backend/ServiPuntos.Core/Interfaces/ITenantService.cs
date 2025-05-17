using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface ITenantService
    {
        Task<IEnumerable<Tenant>> GetAllAsync();
        Task<Tenant?> GetByIdAsync(Guid id);
        Task AddAsync(Tenant tenant);
        Task UpdateAsync(Tenant tenant);
        Task DeleteAsync(Guid id);
    }
}
