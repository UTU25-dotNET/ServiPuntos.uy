using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;

namespace ServiPuntos.Application.Services
{
    public class ProductoUbicacionService : IProductoUbicacionService
    {
        private readonly IProductoUbicacionRepository _iProductoUbicacionRepository;
        private readonly ITenantResolver _iTenantResolver;

        private readonly ITenantContext _iTenantContext;
        public ProductoUbicacionService(IProductoUbicacionRepository productoUbicacionRepository, ITenantContext tenantContext, ITenantResolver tenantResolver)
        {
            _iProductoUbicacionRepository = productoUbicacionRepository;
            _iTenantContext = tenantContext;
            _iTenantResolver = tenantResolver;
        }

        public async Task<IEnumerable<ProductoUbicacion>> GetAllAsync()
        {
            return await _iProductoUbicacionRepository.GetAllAsync();
        }
        public async Task<IEnumerable<ProductoUbicacion>> GetAllAsync(Guid idUbicacion)
        {
            return await _iProductoUbicacionRepository.GetAllAsync(idUbicacion);
        }

        public async Task<ProductoUbicacion?> GetAsync(Guid id)
        {
            return await _iProductoUbicacionRepository.GetAsync(id);
        }
        public async Task AddAsync(ProductoUbicacion productoUbicacion)
        {
            await _iProductoUbicacionRepository.AddAsync(productoUbicacion);
        }
        public async Task UpdateAsync(ProductoUbicacion productoUbicacion)
        {
            await _iProductoUbicacionRepository.UpdateAsync(productoUbicacion);
        }

    }
}