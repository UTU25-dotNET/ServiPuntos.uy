using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Entities;
using ServiPuntos.Infrastructure.Data;

namespace ServiPuntos.Infrastructure.Repositories
{
    public class ProductoCanjeableRepository : IProductoRepository
    {
        private readonly ServiPuntosDbContext _dbContext;

        public ProductoCanjeableRepository(ServiPuntosDbContext context)
        {
            _dbContext = context;
        }

        public Task<ProductoCanjeable?> GetAsync(Guid idProducto)
            => _dbContext.ProductosCanjeables
                .FirstOrDefaultAsync(p => p.Id == idProducto);
        public Task<ProductoCanjeable?> GetAsync(string nombre)
            => _dbContext.ProductosCanjeables
                .FirstOrDefaultAsync(p => p.Nombre == nombre);
        public async Task<IEnumerable<ProductoCanjeable>> GetAllAsync()
            => await _dbContext.ProductosCanjeables
                .ToListAsync();
        public async Task<IEnumerable<ProductoCanjeable>> GetAllAsync(Ubicacion ubicacion)
            => await _dbContext.ProductosCanjeables
                .Include(p => p.DisponibilidadesPorUbicacion)
                .Where(p => p.DisponibilidadesPorUbicacion.Any(d => d.UbicacionId == ubicacion.Id))
                .ToListAsync();
        public Task AddAsync(ProductoCanjeable producto)
        {
            _dbContext.ProductosCanjeables.Add(producto);
            return _dbContext.SaveChangesAsync();
        }
        public Task AddAsync(Ubicacion ubicacion, ProductoCanjeable producto)
        {
            _dbContext.ProductosCanjeables.Add(producto);
            var productoUbicacion = new ProductoUbicacion
            {
                ProductoCanjeableId = producto.Id,
                UbicacionId = ubicacion.Id
            };
            _dbContext.ProductoUbicaciones.Add(productoUbicacion);
            return _dbContext.SaveChangesAsync();
        }
        public Task UpdateAsync(ProductoCanjeable producto)
        {
            _dbContext.ProductosCanjeables.Update(producto);
            return _dbContext.SaveChangesAsync();
        }
        public Task UpdateAsync(Ubicacion ubicacion, ProductoCanjeable producto)
        {
            _dbContext.ProductosCanjeables.Update(producto);
            var productoUbicacion = new ProductoUbicacion
            {
                ProductoCanjeableId = producto.Id,
                UbicacionId = ubicacion.Id
            };
            _dbContext.ProductoUbicaciones.Update(productoUbicacion);
            return _dbContext.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid idProducto)
        {
            var producto = await _dbContext.ProductosCanjeables
                .FirstOrDefaultAsync(p => p.Id == idProducto);
            if (producto != null)
            {
                _dbContext.ProductosCanjeables.Remove(producto);
                await _dbContext.SaveChangesAsync();
            }
        }
        



    }
}
