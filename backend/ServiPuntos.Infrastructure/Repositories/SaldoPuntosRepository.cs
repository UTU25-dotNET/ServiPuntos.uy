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
    public class SaldoPuntosRepository : ISaldoPuntosRepository
    {
        private readonly ServiPuntosDbContext _context;

        public SaldoPuntosRepository(ServiPuntosDbContext context)
        {
            _context = context;
        }

        public async Task<SaldoPuntos> GetByUsuarioIdAndTenantIdAsync(Guid usuarioId, Guid tenantId)
        {
            return await _context.SaldosPuntos
                .Include(s => s.Usuario)
                .Include(s => s.Tenant)
                .FirstOrDefaultAsync(s => s.UsuarioId == usuarioId && s.TenantId == tenantId);
        }

        public async Task<Guid> AddAsync(SaldoPuntos saldoPuntos)
        {
            _context.SaldosPuntos.Add(saldoPuntos);
            await _context.SaveChangesAsync();
            return saldoPuntos.Id;
        }

        public async Task<bool> UpdateAsync(SaldoPuntos saldoPuntos)
        {
            _context.Entry(saldoPuntos).State = EntityState.Modified;
            int result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var saldoPuntos = await _context.SaldosPuntos.FindAsync(id);
            if (saldoPuntos == null)
                return false;

            _context.SaldosPuntos.Remove(saldoPuntos);
            int result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}