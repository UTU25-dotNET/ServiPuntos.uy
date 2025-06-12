using Microsoft.EntityFrameworkCore;
<<<<<<< HEAD
=======

>>>>>>> origin/dev
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

        // En ProductoUbicacionRepository.cs  
public async Task<IEnumerable<ProductoUbicacion>> GetAllAsync()
{
    return await _dbContext.ProductoUbicaciones
        .Include(pu => pu.ProductoCanjeable)  
        .ToListAsync();
}
        public async Task<IEnumerable<ProductoUbicacion>> GetAllAsync(Guid ubicacionId)
<<<<<<< HEAD
{
=======
        {
>>>>>>> origin/dev
    return await _dbContext.ProductoUbicaciones
        .Where(pu => pu.UbicacionId == ubicacionId)
        .Include(pu => pu.ProductoCanjeable)  // ← AGREGAR ESTA LÍNEA
        .ToListAsync();
<<<<<<< HEAD
}
=======
        }

        public async Task<IEnumerable<ProductoUbicacion>> GetAllAsync(Guid ubicacionId, string categoria)
        {
    return await _dbContext.ProductoUbicaciones
        .Where(pu => pu.UbicacionId == ubicacionId && pu.Categoria == categoria)
        .Include(pu => pu.ProductoCanjeable)
        .ToListAsync();
        }
>>>>>>> origin/dev

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