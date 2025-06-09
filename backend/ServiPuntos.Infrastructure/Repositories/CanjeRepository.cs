using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Enums;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiPuntos.Infrastructure.Repositories
{
    public class CanjeRepository : ICanjeRepository
    {
        private readonly ServiPuntosDbContext _context;

        public CanjeRepository(ServiPuntosDbContext context)
        {
            _context = context;
        }

        public async Task<Canje> GetByIdAsync(Guid id)
        {
            return await _context.Canjes
                .Include(c => c.Usuario)
                .Include(c => c.Ubicacion)
                .Include(c => c.Tenant)
                .Include(c => c.ProductoCanjeable)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Canje> GetByCodigoQRAsync(string codigoQR)
        {
            return await _context.Canjes
                .Include(c => c.Usuario)
                .Include(c => c.Ubicacion)
                .Include(c => c.Tenant)
                .Include(c => c.ProductoCanjeable)
                .FirstOrDefaultAsync(c => c.CodigoQR == codigoQR);
        }

        public async Task<IEnumerable<Canje>> GetByUsuarioIdAsync(Guid usuarioId)
        {
            return await _context.Canjes
                .Where(c => c.UsuarioId == usuarioId)
                .Include(c => c.Ubicacion)
                .Include(c => c.ProductoCanjeable)
                .OrderByDescending(c => c.FechaGeneracion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Canje>> GetByUbicacionIdAsync(Guid ubicacionId)
        {
            return await _context.Canjes
                .Where(c => c.UbicacionId == ubicacionId)
                .Include(c => c.Usuario)
                .Include(c => c.ProductoCanjeable)
                .OrderByDescending(c => c.FechaGeneracion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Canje>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _context.Canjes
                .Where(c => c.TenantId == tenantId)
                .Include(c => c.Usuario)
                .Include(c => c.Ubicacion)
                .Include(c => c.ProductoCanjeable)
                .OrderByDescending(c => c.FechaGeneracion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Canje>> GetPendientesByUsuarioIdAsync(Guid usuarioId)
        {
            return await _context.Canjes
                .Where(c => c.UsuarioId == usuarioId && c.Estado == EstadoCanje.Generado && c.FechaExpiracion > DateTime.Now)
                .Include(c => c.Ubicacion)
                .Include(c => c.ProductoCanjeable)
                .OrderByDescending(c => c.FechaGeneracion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Canje>> GetPendientesByUbicacionIdAsync(Guid ubicacionId)
        {
            return await _context.Canjes
                .Where(c => c.UbicacionId == ubicacionId && c.Estado == EstadoCanje.Generado && c.FechaExpiracion > DateTime.Now)
                .Include(c => c.Usuario)
                .Include(c => c.ProductoCanjeable)
                .OrderByDescending(c => c.FechaGeneracion)
                .ToListAsync();
        }

        public async Task<Guid> AddAsync(Canje canje)
        {
            _context.Canjes.Add(canje);
            await _context.SaveChangesAsync();
            return canje.Id;
        }

        public async Task<bool> UpdateAsync(Canje canje)
        {
            _context.Entry(canje).State = EntityState.Modified;
            int result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var canje = await _context.Canjes.FindAsync(id);
            if (canje == null)
                return false;

            _context.Canjes.Remove(canje);
            int result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}