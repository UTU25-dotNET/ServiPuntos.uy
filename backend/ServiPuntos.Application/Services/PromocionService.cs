using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;

namespace ServiPuntos.Application.Services
{
    public class PromocionService : IPromocionService
    {
        private readonly IPromocionRepository _repository;
        public PromocionService(IPromocionRepository repository)
        {
            _repository = repository;
        }

        public Task<Promocion?> GetPromocionAsync(Guid id) => _repository.GetAsync(id);

        public Task<IEnumerable<Promocion>> GetPromocionesByTenantAsync(Guid tenantId) => _repository.GetAllByTenantAsync(tenantId);

        public Task AddPromocionAsync(Promocion promocion) => _repository.AddAsync(promocion);

        public Task UpdatePromocionAsync(Promocion promocion) => _repository.UpdateAsync(promocion);

        public Task DeletePromocionAsync(Guid id) => _repository.DeleteAsync(id);
    }
}