using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiPuntos.Core.Entities;

namespace ServiPuntos.Core.Interfaces
{
    public interface ICanjeRepository
    {
        Task<Canje> GetByIdAsync(int id);
        Task<Canje> GetByCodigoQRAsync(string codigoQR);
        Task<IEnumerable<Canje>> GetByUsuarioIdAsync(int usuarioId);
        Task<IEnumerable<Canje>> GetByUbicacionIdAsync(int ubicacionId);
        Task<IEnumerable<Canje>> GetByTenantIdAsync(int tenantId);
        Task<IEnumerable<Canje>> GetPendientesByUsuarioIdAsync(int usuarioId);
        Task<int> AddAsync(Canje canje);
        Task<bool> UpdateAsync(Canje canje);
        Task<bool> DeleteAsync(int id);
    }
}