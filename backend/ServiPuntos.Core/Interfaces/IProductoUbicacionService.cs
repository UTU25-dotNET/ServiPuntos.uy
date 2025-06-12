using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface IProductoUbicacionService
    {
        Task<IEnumerable<ProductoUbicacion>> GetAllAsync();
        Task<IEnumerable<ProductoUbicacion>> GetAllAsync(Guid ubicacionId);

<<<<<<< HEAD
=======
        Task<IEnumerable<ProductoUbicacion>> GetAllAsync(Guid idUbicacion, string categoria);

>>>>>>> origin/dev
        Task<ProductoUbicacion?> GetAsync(Guid id);
        Task AddAsync(ProductoUbicacion productoUbicacion);
        Task UpdateAsync(ProductoUbicacion productoUbicacion);
    }
}