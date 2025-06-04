// Namespace: ServiPuntos.Infrastructure.Data.Repositories
// Propósito: Implementación de IAudienciaRepository usando Entity Framework Core.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // Necesario para EF Core
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;
// Asume que tienes un DbContext llamado AppDbContext
 using ServiPuntos.Infrastructure.Data; 

namespace ServiPuntos.Infrastructure.Repositories
{
    public class AudienciaRepository : IAudienciaRepository // Implementa tu interfaz
    {
        private readonly ServiPuntosDbContext _dbContext; // Tu DbContext

        public AudienciaRepository(ServiPuntosDbContext dbContext) // Inyectar DbContext
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<Audiencia?> GetByIdWithReglasAsync(Guid audienciaId)
        {
            return await _dbContext.Audiencias
                                 .Include(a => a.Reglas) // Eager loading de las reglas
                                 .FirstOrDefaultAsync(a => a.Id == audienciaId);
        }

        public async Task<Audiencia?> GetByNombreUnicoWithReglasAsync(Guid tenantId, string nombreUnicoInterno)
        {
            return await _dbContext.Audiencias
                                 .Include(a => a.Reglas)
                                 .FirstOrDefaultAsync(a => a.TenantId == tenantId &&
                                                           a.NombreUnicoInterno.ToLower() == nombreUnicoInterno.ToLower());
        }

        public async Task<IEnumerable<Audiencia>> ListByTenantIdWithReglasAsync(Guid tenantId, bool soloActivas = false, bool ordenarPorPrioridad = false)
        {
            var query = _dbContext.Audiencias
                                .Include(a => a.Reglas)
                                .Where(a => a.TenantId == tenantId);

            if (soloActivas)
            {
                query = query.Where(a => a.Activa);
            }

            if (ordenarPorPrioridad)
            {
                query = query.OrderBy(a => a.Prioridad);
            }
            else
            {
                query = query.OrderBy(a => a.NombreDescriptivo); // Un orden por defecto
            }

            return await query.ToListAsync();
        }

        public async Task AddAsync(Audiencia audiencia)
        {
            // EF Core maneja la inserción de la entidad principal y sus entidades hijas (Reglas)
            // si la relación está bien configurada y Reglas es una nueva colección.
            await _dbContext.Audiencias.AddAsync(audiencia);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Audiencia audiencia)
        {
            // La parte más compleja: sincronizar la colección de Reglas.
            // EF Core puede hacer esto si cargas la entidad existente con sus reglas,
            // luego modificas la colección y guardas.

            var audienciaExistente = await _dbContext.Audiencias
                                                 .Include(a => a.Reglas)
                                                 .FirstOrDefaultAsync(a => a.Id == audiencia.Id && a.TenantId == audiencia.TenantId);

            if (audienciaExistente == null)
            {
                // Manejar el caso donde la audiencia no existe (podría ser una excepción)
                throw new KeyNotFoundException($"Audiencia con ID {audiencia.Id} no encontrada para actualizar.");
            }

            // Actualizar propiedades escalares de Audiencia
            _dbContext.Entry(audienciaExistente).CurrentValues.SetValues(audiencia);
            audienciaExistente.FechaModificacion = DateTime.UtcNow; // Asegurar que se actualice

            // Sincronizar la colección de Reglas
            // 1. Eliminar reglas que ya no están en la nueva colección
            var reglasAEliminar = audienciaExistente.Reglas
                .Where(rDb => !audiencia.Reglas.Any(rInput => rInput.Id != Guid.Empty && rInput.Id == rDb.Id)) // Id=0 son nuevas
                .ToList();
            foreach (var reglaEliminar in reglasAEliminar)
            {
                _dbContext.ReglasAudiencia.Remove(reglaEliminar);
            }

            // 2. Actualizar reglas existentes y añadir nuevas
            foreach (var reglaInput in audiencia.Reglas)
            {
                var reglaExistente = audienciaExistente.Reglas
                    .FirstOrDefault(rDb => rDb.Id != Guid.Empty && rDb.Id == reglaInput.Id);

                if (reglaExistente != null) // Actualizar regla existente
                {
                    _dbContext.Entry(reglaExistente).CurrentValues.SetValues(reglaInput);
                }
                else // Añadir nueva regla
                {
                    // Asegurar que AudienciaId está seteado si es una regla completamente nueva
                    reglaInput.AudienciaId = audienciaExistente.Id;
                    audienciaExistente.Reglas.Add(reglaInput); // EF Core la rastreará como nueva
                }
            }

            await _dbContext.SaveChangesAsync();
        }


        public async Task DeleteAsync(Guid audienciaId)
        {
            var audiencia = await GetByIdWithReglasAsync(audienciaId); // Cargar con reglas para borrado en cascada por EF
            if (audiencia != null)
            {
                // Si las reglas no se borran en cascada por configuración de BD/EF, borrarlas manualmente:
                // _dbContext.ReglasAudiencia.RemoveRange(audiencia.Reglas);
                _dbContext.Audiencias.Remove(audiencia);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<bool> ExisteNombreUnicoAsync(Guid tenantId, string nombreUnicoInterno, Guid? excluirAudienciaId = null)
        {
            var query = _dbContext.Audiencias
                .Where(a => a.TenantId == tenantId &&
                            a.NombreUnicoInterno.ToLower() == nombreUnicoInterno.ToLower());

            if (excluirAudienciaId.HasValue)
            {
                query = query.Where(a => a.Id != excluirAudienciaId.Value);
            }

            return await query.AnyAsync();
        }
    }
}
