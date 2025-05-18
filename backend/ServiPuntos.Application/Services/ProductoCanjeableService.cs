using ServiPuntos.Core.Interfaces;
using ServiPuntos.Core.Entities;

namespace ServiPuntos.Application.Services
{
    public class ProductoCanjeableService : IProductoCanjeableService
    {
        private readonly IProductoCanjeableRepository _iProductoCanjeableRepository;

        private readonly ITenantResolver _iTenantResolver;

        private readonly ITenantContext _iTenantContext;
        public ProductoCanjeableService(IProductoCanjeableRepository iProductoRepository, ITenantResolver iTenantResolver, ITenantContext tenantContext)
        {
            _iProductoCanjeableRepository = iProductoRepository;

            _iTenantResolver = iTenantResolver;
            _iTenantContext = tenantContext;
        }
        //GET
        public async Task<ProductoCanjeable?> GetProductoAsync(Guid idProducto)
        {
            return await _iProductoCanjeableRepository.GetAsync(idProducto);
        }
        public async Task<ProductoCanjeable?> GetProductoAsync(string nombre)
        {
            return await _iProductoCanjeableRepository.GetAsync(nombre);
        }
        //GET ALL
        public async Task<IEnumerable<ProductoCanjeable>> GetAllProductosAsync()
        {
            return await _iProductoCanjeableRepository.GetAllAsync();
        }
        public async Task<IEnumerable<ProductoCanjeable>> GetAllProductosAsync(Ubicacion ubicacion)
        {
            return await _iProductoCanjeableRepository.GetAllAsync(ubicacion);
        }

        //ADD
        public async Task AddProductoAsync(ProductoCanjeable producto)
        {
            await _iProductoCanjeableRepository.AddAsync(producto);
        }
        public async Task AddProductoAsync(Ubicacion ubicacion, ProductoCanjeable producto)
        {
            await _iProductoCanjeableRepository.AddAsync(ubicacion, producto);
        }

        //UPDATE
        public async Task UpdateProductoAsync(ProductoCanjeable producto)
        {
            await _iProductoCanjeableRepository.UpdateAsync(producto);
        }
        public async Task UpdateProductoAsync(Ubicacion ubicacion, ProductoCanjeable producto)
        {
            await _iProductoCanjeableRepository.UpdateAsync(ubicacion, producto);
        }
        //DELETE
        public async Task DeleteProductoAsync(Guid idProducto)
        {
            await _iProductoCanjeableRepository.DeleteAsync(idProducto);
        }
        public async Task DeleteProductoAsync(Ubicacion ubicacion, Guid idProducto)
        {
            await _iProductoCanjeableRepository.DeleteAsync(ubicacion, idProducto);
        }

    }
}
