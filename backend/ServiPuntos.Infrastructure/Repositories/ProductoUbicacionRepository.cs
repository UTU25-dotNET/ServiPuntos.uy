using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.Data;

namespace ServiPuntos.Infrastructure.Repositories
{
    public class ProductoUbicacionRepository : IProductoUbicacionRepository
    {
        private readonly ServiPuntosDbContext _dbContext;
        public ProductoUbicacionRepository(ServiPuntosDbContext context)
        {
            _dbContext = context;
        }
        public Task<ProductoUbicacion?> GetAsync(Guid id)
            => _dbContext.ProductoUbicaciones
                .FirstOrDefaultAsync(p => p.Id == id);

        /*public Task<IEnumerable<ProductoUbicacion>> GetAllAsync()
            => _dbContext.ProductoUbicaciones
                .ToListAsync();*/
        public Task AddAsync(ProductoUbicacion productoUbicacion)
        {
            _dbContext.ProductoUbicaciones.Add(productoUbicacion);
            return _dbContext.SaveChangesAsync();
        }
        public Task UpdateAsync(ProductoUbicacion productoUbicacion)
        {
            _dbContext.ProductoUbicaciones.Update(productoUbicacion);
            return _dbContext.SaveChangesAsync();
        }

    }
}
