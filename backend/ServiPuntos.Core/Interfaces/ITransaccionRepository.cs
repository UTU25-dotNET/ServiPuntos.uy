using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface ITransaccionRepository
    {
        Task<Transaccion> GetByIdAsync(Guid id);
        Task<IEnumerable<Transaccion>> GetByUsuarioIdAsync(Guid usuarioId);
        Task<IEnumerable<Transaccion>> GetByUbicacionIdAsync(Guid ubicacionId);
        Task<IEnumerable<Transaccion>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Transaccion>> GetByDateRangeAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<Guid> AddAsync(Transaccion transaccion);
        Task<bool> UpdateAsync(Transaccion transaccion);
        Task<bool> DeleteAsync(Guid id);
    }
}