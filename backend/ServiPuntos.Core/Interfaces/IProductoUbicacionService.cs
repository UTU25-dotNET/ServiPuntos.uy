using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface IProductoUbicacionService
    {
        Task<IEnumerable<ProductoUbicacion>> GetAllAsync();
        Task<ProductoUbicacion?> GetAsync(Guid id);
        Task AddAsync(ProductoUbicacion productoUbicacion);
        Task UpdateAsync(ProductoUbicacion productoUbicacion);
    }
}
