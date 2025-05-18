using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface IProductoCanjeableRepository
    {
        Task<ProductoCanjeable?> GetAsync(Guid idProducto);
        Task<ProductoCanjeable?> GetAsync(string nombre);
        Task<IEnumerable<ProductoCanjeable>> GetAllAsync();
        Task<IEnumerable<ProductoCanjeable>> GetAllAsync(Ubicacion ubicacion);
        Task AddAsync(ProductoCanjeable producto);
        Task AddAsync(Ubicacion ubicacion, ProductoCanjeable producto);
        Task UpdateAsync(ProductoCanjeable producto);
        Task UpdateAsync(Ubicacion ubicacion, ProductoCanjeable producto);
        Task DeleteAsync(Guid idProducto);
        Task DeleteAsync(Ubicacion ubicacion, Guid idProducto);

    }
}
