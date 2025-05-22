using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface IUbicacionRepository
    {
        Task<Ubicacion?> GetAsync(Guid idUbicacion);
        Task<Ubicacion?> GetAsync(string nombre);
        Task<Ubicacion?> GetAsync(Guid tenantId, Guid idUbicacion);

        Task<IEnumerable<Ubicacion>> GetAllAsync();
        Task<IEnumerable<Ubicacion>> GetAllAsync(Guid tenantId);

        Task AddAsync(Ubicacion ubicacion);
        Task AddAsync(Guid tenantId, Ubicacion ubicacion);

        Task UpdateAsync(Ubicacion ubicacion);
        Task UpdateAsync(Guid tenantId, Ubicacion ubicacion);

        Task DeleteAsync(Guid idUbicacion);
        Task DeleteAsync(Guid tenantId, Guid idUbicacion);

    }
}
