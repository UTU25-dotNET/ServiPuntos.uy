using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface IProductoUbicacionRepository
    {
        Task<ProductoUbicacion?> GetAsync(Guid id);
        Task<IEnumerable<ProductoUbicacion>> GetAllAsync();
        Task<IEnumerable<ProductoUbicacion>> GetAllAsync(Guid idUbicacion);
        Task AddAsync(ProductoUbicacion productoUbicacion);
        Task UpdateAsync(ProductoUbicacion productoUbicacion);
    }
}