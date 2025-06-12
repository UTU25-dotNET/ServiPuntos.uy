using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Refit;
using ServiPuntos.Mobile.Models;

namespace ServiPuntos.Mobile.Services
{
    // Interfaz Refit para llamar al endpoint
    public interface IProductoApi
    {
        // GET api/productoUbicacion/ubicacion/{ubicacionId}
        [Get("/ubicacion/{ubicacionId}")]
        Task<List<ProductoUbicacionDto>> GetByUbicacionAsync(string ubicacionId);
    }

    // Servicio y su interfaz combinados
    public interface IProductoService
    {
        Task<List<ProductoUbicacionDto>> GetProductosPorUbicacionAsync(string ubicacionId);
        Task<int> GetStockAsync(string ubicacionId, string productoCanjeableId);
    }

    public class ProductoService : IProductoService
    {
        private readonly IProductoApi _api;

        public ProductoService(IProductoApi api)
        {
            _api = api;
        }

        public async Task<List<ProductoUbicacionDto>> GetProductosPorUbicacionAsync(string ubicacionId)
        {
            var productos = await _api.GetByUbicacionAsync(ubicacionId);
            return productos ?? new List<ProductoUbicacionDto>();
        }

        public async Task<int> GetStockAsync(string ubicacionId, string productoCanjeableId)
        {
            if (!Guid.TryParse(productoCanjeableId, out var pid))
                throw new ArgumentException("ID de producto no válido", nameof(productoCanjeableId));

            var productos = await GetProductosPorUbicacionAsync(ubicacionId);
            var item = productos.FirstOrDefault(p => p.ProductoCanjeableId == pid);
            return item?.StockDisponible ?? 0;
        }
    }
}
