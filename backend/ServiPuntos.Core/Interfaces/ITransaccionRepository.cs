using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.DTOs;

namespace ServiPuntos.Core.Interfaces
{
    public interface ITransaccionRepository
    {
        Task<Transaccion?> GetByIdAsync(Guid id);
        Task<IEnumerable<Transaccion>> GetByUsuarioIdAsync(Guid usuarioId);
        Task<IEnumerable<Transaccion>> GetByUbicacionIdAsync(Guid ubicacionId);
         Task<IEnumerable<Transaccion>> GetByUsuarioIdPaginatedAsync(Guid usuarioId, Guid? cursor, int limit);
        Task<IEnumerable<Transaccion>> GetByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Transaccion>> GetByDateRangeAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<Guid> AddAsync(Transaccion transaccion);
        Task<bool> UpdateAsync(Transaccion transaccion);
        Task<bool> DeleteAsync(Guid id);
        Task<Transaccion?> GetByPayPalPaymentIdAsync(string pagoPayPalId);

        /// <summary>
        /// Obtiene agregados de transacciones para un usuario específico.
        /// </summary>
        Task<DatosTransaccionesUsuario> GetAggregatesByUsuarioIdAsync(Guid usuarioId);

        /// <summary>
        /// Obtiene la suma total de dinero generado por todas las transacciones.
        /// </summary>
        Task<decimal> GetMontoTotalAsync();
    }
}