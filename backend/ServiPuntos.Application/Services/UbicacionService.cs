using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;

namespace ServiPuntos.Application.Services
{
    public class UbicacionService : IUbicacionService
    {
        private readonly IUbicacionRepository _iUbicacionRepository;

        private readonly ITenantResolver _iTenantResolver;

        private readonly ITenantContext _iTenantContext;
        public UbicacionService(IUbicacionRepository iUbicacionRepository, ITenantResolver iTenantResolver, ITenantContext tenantContext)
        {
            _iUbicacionRepository = iUbicacionRepository;

            _iTenantResolver = iTenantResolver;
            _iTenantContext = tenantContext;
        }
        //GET
        public async Task<Ubicacion?> GetUbicacionAsync(Guid idUbicacion)
        {
            return await _iUbicacionRepository.GetAsync(idUbicacion);
        }
        public async Task<Ubicacion?> GetUbicacionAsync(string nombre)
        {
            return await _iUbicacionRepository.GetAsync(nombre);
        }
        public async Task<Ubicacion?> GetUbicacionAsync(Guid tenantId, Guid idUbicacion)
        {
            return await _iUbicacionRepository.GetAsync(tenantId, idUbicacion);
        }
        //GET ALL
        public async Task<IEnumerable<Ubicacion>> GetAllUbicacionesAsync()
        {
            return await _iUbicacionRepository.GetAllAsync();
        }
        public async Task<IEnumerable<Ubicacion>> GetAllUbicacionesAsync(Guid tenantId)
        {
            return await _iUbicacionRepository.GetAllAsync(tenantId);
        }
        //ADD
        public async Task AddUbicacionAsync(Ubicacion ubicacion)
        {
            await _iUbicacionRepository.AddAsync(ubicacion);
        }
        public async Task AddUbicacionAsync(Guid tenantId, Ubicacion ubicacion)
        {
            await _iUbicacionRepository.AddAsync(tenantId, ubicacion);
        }
        //UPDATE
        public async Task UpdateUbicacionAsync(Ubicacion ubicacion)
        {
            await _iUbicacionRepository.UpdateAsync(ubicacion);
        }
        public async Task UpdateUbicacionByTenantAsync(Guid tenantId, Ubicacion ubicacion)
        {
            await _iUbicacionRepository.UpdateAsync(tenantId, ubicacion);
        }
        //DELETE
        public async Task DeleteUbicacionAsync(Guid idUbicacion)
        {
            await _iUbicacionRepository.DeleteAsync(idUbicacion);
        }
        public async Task DeleteUbicacionAsync(Guid tenantId, Guid idUbicacion)
        {
            await _iUbicacionRepository.DeleteAsync(tenantId, idUbicacion);
        }
    }
}
