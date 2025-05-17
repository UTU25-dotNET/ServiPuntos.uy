/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Core.Entities;

namespace ServiPuntos.Application.Services
{
    public class ProductoService : IProductoService
    {
        private readonly IProductoRepository _iProductoRepository;

        private readonly ITenantResolver _iTenantResolver;

        private readonly ITenantContext _iTenantContext;
        public ProductoService(IUbicacionRepository iUbicacionRepository, ITenantResolver iTenantResolver, ITenantContext tenantContext)
        {
            _iProductoRepository = iUbicacionRepository;

            _iTenantResolver = iTenantResolver;
            _iTenantContext = tenantContext;
        }
        //GET
        public async Task<Producto?> GetProductoAsync(Guid idProducto)
        {
            return await _iProductoRepository.GetAsync(idProducto);
        }
        public async Task<Producto?> GetProductoAsync(string nombre)
        {
            return await _iProductoRepository.GetAsync(nombre);
        }
        public async Task<Producto?> GetProductoAsync(Guid tenantId, Guid idProducto)
        {
            return await _iProductoRepository.GetAsync(tenantId, idProducto);
        }
        //GET ALL
        public async Task<IEnumerable<Producto>> GetAllProductosAsync()
        {
            return await _iProductoRepository.GetAllAsync();
        }
        public async Task<IEnumerable<Producto>> GetAllProductosAsync(Ubicacion ubicacion)
        {
            return await _iProductoRepository.GetAllAsync(ubicacion);
        }
        public async Task<IEnumerable<Producto>> GetAllProductosAsync(Guid tenantId)
        {
            return await _iProductoRepository.GetAllAsync(tenantId);
        }
        //ADD
        public async Task AddProductoAsync(Producto producto)
        {
            await _iProductoRepository.AddAsync(producto);
        }
        public async Task AddProductoAsync(Ubicacion ubicacion, Producto producto)
        {
            await _iProductoRepository.AddAsync(ubicacion, producto);
        }

    }
}*/
