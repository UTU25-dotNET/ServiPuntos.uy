using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface ITransaccionRepository
    {
        Task<Transaccion> GetByIdAsync(int id);
        Task<IEnumerable<Transaccion>> GetByUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<Transaccion>> GetByUbicacionIdAsync(int ubicacionId);
        Task<IEnumerable<Transaccion>> GetByTenantIdAsync(int tenantId);
        Task<IEnumerable<Transaccion>> GetByDateRangeAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<int> AddAsync(Transaccion transaccion);
        Task<bool> UpdateAsync(Transaccion transaccion);
        Task<bool> DeleteAsync(int id);
    }
}