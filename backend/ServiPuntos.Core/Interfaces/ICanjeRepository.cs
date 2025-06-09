using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface ICanjeRepository
    {
        Task<Canje> GetByIdAsync(Guid id);
        Task<Canje> GetByCodigoQRAsync(string codigoQR);
        Task<IEnumerable<Canje>> GetByUsuarioIdAsync(Guid usuarioId);
        Task<IEnumerable<Canje>> GetByUbicacionIdAsync(Guid ubicacionId);
        Task<IEnumerable<Canje>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Canje>> GetPendientesByUsuarioIdAsync(Guid usuarioId);
        Task<IEnumerable<Canje>> GetPendientesByUbicacionIdAsync(Guid ubicacionId);
        Task<Guid> AddAsync(Canje canje);
        Task<bool> UpdateAsync(Canje canje);
        Task<bool> DeleteAsync(int id);
    }
}