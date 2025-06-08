using Microsoft.EntityFrameworkCore;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.Interfaces;
using ServiPuntos.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiPuntos.Infrastructure.Repositories
{
    public class TransaccionRepository : ITransaccionRepository
    {
        private readonly ServiPuntosDbContext _context;

        public TransaccionRepository(ServiPuntosDbContext context)
        {
            _context = context;
        }

        public async Task<Transaccion> GetByIdAsync(Guid id)
        {
            return await _context.Transacciones
                .Include(t => t.Usuario)
                .Include(t => t.Ubicacion)
                .Include(t => t.Tenant)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<Transaccion>> GetByUsuarioIdAsync(Guid usuarioId)
        {
            return await _context.Transacciones
                .Where(t => t.UsuarioId == usuarioId)
                .Include(t => t.Ubicacion)
                .OrderByDescending(t => t.FechaTransaccion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaccion>> GetByUbicacionIdAsync(Guid ubicacionId)
        {
            return await _context.Transacciones
                .Where(t => t.UbicacionId == ubicacionId)
                .Include(t => t.Usuario)
                .OrderByDescending(t => t.FechaTransaccion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaccion>> GetByTenantIdAsync(Guid tenantId)
        {
            return await _context.Transacciones
                .Where(t => t.TenantId == tenantId)
                .Include(t => t.Usuario)
                .Include(t => t.Ubicacion)
                .OrderByDescending(t => t.FechaTransaccion)
                .ToListAsync();
        }

        public async Task<IEnumerable<Transaccion>> GetByDateRangeAsync(DateTime fechaInicio, DateTime fechaFin)
        {
            return await _context.Transacciones
                .Where(t => t.FechaTransaccion >= fechaInicio && t.FechaTransaccion <= fechaFin)
                .Include(t => t.Usuario)
                .Include(t => t.Ubicacion)
                .Include(t => t.Tenant)
                .OrderByDescending(t => t.FechaTransaccion)
                .ToListAsync();
        }

        public async Task<Guid> AddAsync(Transaccion transaccion)
        {
            _context.Transacciones.Add(transaccion);
            await _context.SaveChangesAsync();
            return transaccion.Id;
        }

        public async Task<bool> UpdateAsync(Transaccion transaccion)
        {
            _context.Entry(transaccion).State = EntityState.Modified;
            int result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var transaccion = await _context.Transacciones.FindAsync(id);
            if (transaccion == null)
                return false;

            _context.Transacciones.Remove(transaccion);
            int result = await _context.SaveChangesAsync();
            return result > 0;
        }
        public async Task<Transaccion> GetByPayPalPaymentIdAsync(string pagoPayPalId)
        {
            return await _context.Transacciones
                .Include(t => t.Usuario)
                .Include(t => t.Ubicacion)
                .Include(t => t.Tenant)
                .FirstOrDefaultAsync(t => t.PagoPayPalId == pagoPayPalId);
        }
    }
}