using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ServiPuntos.Core.Entities;
using ServiPuntos.Core.NAFTA;

namespace ServiPuntos.Core.Interfaces
{
    public interface ITransaccionService
    {
        Task<Transaccion?> GetTransaccionByIdAsync(Guid id);
        Task<IEnumerable<Transaccion>> GetTransaccionesByUsuarioIdAsync(Guid usuarioId);

        Task<IEnumerable<Transaccion>> GetTransaccionesByUsuarioIdPaginatedAsync(Guid usuarioId, Guid? cursor, int limit);
        Task<IEnumerable<Transaccion>> GetTransaccionesByUbicacionIdAsync(Guid ubicacionId);
        Task<IEnumerable<Transaccion>> GetTransaccionesByTenantIdAsync(Guid tenantId);
        Task<IEnumerable<Transaccion>> GetTransaccionesByDateRangeAsync(DateTime fechaInicio, DateTime fechaFin);
        Task<RespuestaPuntosNAFTA> ProcesarTransaccionNAFTAAsync(TransaccionNAFTA transaccion, Guid tenantId, Guid ubicacionId);
        Task<Guid> RegistrarTransaccionAsync(Transaccion transaccion);
    }
}