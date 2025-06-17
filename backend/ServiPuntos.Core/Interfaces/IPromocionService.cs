using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface IPromocionService
    {
        Task<Promocion?> GetPromocionAsync(Guid id);
        Task<IEnumerable<Promocion>> GetPromocionesByTenantAsync(Guid tenantId);
        Task AddPromocionAsync(Promocion promocion);
        Task UpdatePromocionAsync(Promocion promocion);
        Task DeletePromocionAsync(Guid id);
    }
}