using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface IPromocionRepository
    {
        Task<Promocion?> GetAsync(Guid id);
        Task<IEnumerable<Promocion>> GetAllAsync();
        Task<IEnumerable<Promocion>> GetAllByTenantAsync(Guid tenantId);
        Task AddAsync(Promocion promocion);
        Task UpdateAsync(Promocion promocion);
        Task DeleteAsync(Guid id);
    }
}