using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface IProductoCanjeableService
    {
        Task<ProductoCanjeable?> GetProductoAsync(Guid idProducto);
        Task<ProductoCanjeable?> GetProductoAsync(string nombre);
        Task<IEnumerable<ProductoCanjeable>> GetAllProductosAsync();
        Task<IEnumerable<ProductoCanjeable>> GetAllProductosAsync(Ubicacion ubicacion);
        Task AddProductoAsync(ProductoCanjeable producto);
        Task AddProductoAsync(Ubicacion ubicacion, ProductoCanjeable producto);
        Task UpdateProductoAsync(ProductoCanjeable producto);
        Task UpdateProductoAsync(Ubicacion ubicacion, ProductoCanjeable producto);
        Task DeleteProductoAsync(Guid idProducto);
        Task DeleteProductoAsync(Ubicacion ubicacion, Guid idProducto);
    }
}
