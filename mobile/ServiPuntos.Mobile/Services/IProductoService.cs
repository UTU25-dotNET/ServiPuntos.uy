
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    public interface IProductoService
    {

        Task<List<ProductoCanjeableDto>> GetProductosPorUbicacionAsync(string ubicacionId);


        Task<int> GetStockAsync(string ubicacionId, string productoId);
    }
}
